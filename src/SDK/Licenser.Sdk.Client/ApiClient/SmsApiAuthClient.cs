using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;
using Licenser.Sdk.Client.Abstractions;
using Licenser.Shared.Exceptions;
using Licenser.Shared.Models;

namespace Licenser.Sdk.Client
{
    public class SmsApiAuthClient : ISmsApiAuthClient
    {
        #region Fields

        protected readonly HttpClient _client;
        protected readonly SmsApiClientOptions ClientOptions;
        protected readonly JsonSerializerOptions _serializerOptions;
        protected UserAdvancedCredentials _userCredentials;
        private TokenResponse _accessToken;

        #endregion

        /// <summary>
        /// Creates new instance of SmsApiAuthClient which implements ISmsApiAuthClient
        /// </summary>
        /// <param name="clientOptions">Client settings</param>
        /// <param name="apiVersion">SMS API version</param>
        /// <exception cref="ArgumentNullException">Throws if SmsApiClientOptions is null</exception>
        // ReSharper disable once MemberCanBeProtected.Global
        public SmsApiAuthClient(SmsApiClientOptions clientOptions, ApiVersion apiVersion = ApiVersion.V1)
        {
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }

            if (string.IsNullOrEmpty(clientOptions.ApiServerAddress))
            {
                throw new ArgumentNullException(nameof(clientOptions.ApiServerAddress));
            }

            if (string.IsNullOrEmpty(clientOptions.IdentityServerAddress))
            {
                throw new ArgumentNullException(nameof(clientOptions.IdentityServerAddress));
            }

            ClientOptions = clientOptions;
            _client = new HttpClient()
            {
                BaseAddress = new Uri($"{ClientOptions.ApiServerAddress}/{apiVersion}/")
            };

            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        #region Authentication methods

        /// <inheritdoc/>
        public bool IsAuthenticated()
        {
            return _client.DefaultRequestHeaders.Authorization != null;
        }

        /// <inheritdoc/>
        public void SetAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) 
                return;

            _client.SetBearerToken(accessToken);
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<TokenResponse>> AuthenticatePasswordAsync(UserAdvancedCredentials userCredentials)
        {
            if (userCredentials == null)
                throw new ArgumentNullException(nameof(UserAdvancedCredentials));
            
            var discovery = await _client.GetDiscoveryDocumentAsync(ClientOptions.IdentityServerAddress);

            if (discovery.IsError)
            {
                return new ApiResponse<TokenResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {discovery.Error}",
                    Data = null
                };
            }

            var passwordRequest = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = discovery.TokenEndpoint,
                Scope = ClientOptions.Scope,
                ClientId = ClientOptions.ClientId,
                ClientSecret = ClientOptions.ClientSecret,
                Password = userCredentials.Password,
                UserName = userCredentials.UserName
            });

            if (passwordRequest.IsError)
            {
                return new ApiResponse<TokenResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {passwordRequest.Error}, error_desc: {passwordRequest.ErrorDescription}",
                    Data = passwordRequest
                };
            }
            
            SetAccessToken(passwordRequest.AccessToken);
            var userInfo = await _client.GetUserInfoAsync(new UserInfoRequest()
            {
                Address = discovery.UserInfoEndpoint,
                Token = passwordRequest.AccessToken
            });
            
            _accessToken = passwordRequest;
            _userCredentials = userCredentials;
            _userCredentials.UserName = userInfo.Claims.FirstOrDefault(i => i.Type == "name")?.Value;
            _userCredentials.Email = userInfo.Claims.FirstOrDefault(i => i.Type == "email")?.Value;
            _userCredentials.Id = userInfo.Claims.FirstOrDefault(i => i.Type == "sub")?.Value;

            return new ApiResponse<TokenResponse>()
            {
                Message = "Returned access token",
                Data = _accessToken,
                Status = ApiResponseStatus.Success
            };
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new ApiResponse<TokenResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "auth_error: refresh token is empty",
                    Data = null
                };
            }

            var discovery = await _client.GetDiscoveryDocumentAsync(ClientOptions.IdentityServerAddress);

            if (discovery.IsError)
            {
                return new ApiResponse<TokenResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {discovery.Error}",
                    Data = null
                };
            }

            var refreshTokenRequest = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                Address = discovery.TokenEndpoint,
                ClientId = ClientOptions.ClientId,
                ClientSecret = ClientOptions.ClientSecret,
                RefreshToken = refreshToken,
                Scope = ClientOptions.Scope
            });

            if (refreshTokenRequest.IsError)
            {
                return new ApiResponse<TokenResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {refreshTokenRequest.Error}, error_desc: {refreshTokenRequest.ErrorDescription}",
                    Data = refreshTokenRequest
                };
            }

            _accessToken = refreshTokenRequest;
            SetAccessToken(refreshTokenRequest.AccessToken);

            return new ApiResponse<TokenResponse>()
            {
                Message = "Returned access token",
                Data = _accessToken,
                Status = ApiResponseStatus.Success
            }; 
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<TokenRevocationResponse>> RevokeToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ApiResponse<TokenRevocationResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "auth_error: accessToken is empty",
                    Data = null
                };
            }

            var discovery = await _client.GetDiscoveryDocumentAsync(ClientOptions.IdentityServerAddress);

            if (discovery.IsError)
            {
                return new ApiResponse<TokenRevocationResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {discovery.Error}",
                    Data = null
                };
            }

            var tokenRevocationResponse = await _client.RevokeTokenAsync(new TokenRevocationRequest()
            {
                Address = discovery.RevocationEndpoint,
                ClientId = ClientOptions.ClientId,
                ClientSecret = ClientOptions.ClientSecret,
                Token = accessToken
            });

            if (tokenRevocationResponse.IsError)
            {
                return new ApiResponse<TokenRevocationResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"auth_error: {tokenRevocationResponse.Error}",
                    Data = tokenRevocationResponse
                };
            }

            return new ApiResponse<TokenRevocationResponse>()
            {
                Status = ApiResponseStatus.Success,
                Message = "Token revoked",
                Data = tokenRevocationResponse
            };
        }

        #endregion

        #region Request helper methods

        protected async Task<ApiResponse<T>> GetRequestAsync<T>(string requestRoute)
        {
            if (!IsAuthenticated())
            {
                return new ApiResponse<T>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "error: client was not authorized"
                };
            }

            try
            {
                var response = await _client.GetAsync(requestRoute);
                var apiResponse = await GetResponseContentAsync<ApiResponse<T>>(response);
                return apiResponse;
            }
            catch (ServerErrorException e)
            {
                return new ApiResponse<T>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Server Internal Error: {e}"
                };
            }
            catch (HttpRequestException)
            {
                return new ApiResponse<T>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Maybe currently API server is not available."
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<T>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = e.Message
                };
            }
        }

        protected async Task<ApiResponse> PostRequestAsync<T>(string requestRoute, T requestBody)
        {
            if (!IsAuthenticated())
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "error: client was not authorized"
                };
            }

            try
            {
                var content = GetJsonContent(requestBody);
                var response = await _client.PostAsync(requestRoute, content);
                return await GetResponseContentAsync<ApiResponse>(response);
            }
            catch (ServerErrorException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Server Internal Error: {e}",
                    Data = e
                };
            }
            catch (HttpRequestException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Maybe currently API server is not available.",
                    Data = e
                };
            }
            catch (Exception e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = e.Message,
                    Data = e
                };
            }
        }

        protected async Task<ApiResponse<TResponse>> PostRequestAsync<TResponse, TBody>(string requestRoute, TBody requestBody)
        {
            if (!IsAuthenticated())
            {
                return new ApiResponse<TResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "error: client was not authorized"
                };
            }

            try
            {
                var content = GetJsonContent(requestBody);
                var response = await _client.PostAsync(requestRoute, content);
                return await GetResponseContentAsync<ApiResponse<TResponse>>(response);
            }
            catch (ServerErrorException e)
            {
                return new ApiResponse<TResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Server Internal Error: {e}"
                };
            }
            catch (HttpRequestException)
            {
                return new ApiResponse<TResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Maybe currently API server is not available."
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<TResponse>()
                {
                    Status = ApiResponseStatus.Error,
                    Message = e.Message
                };
            }
        }

        protected async Task<ApiResponse> PutRequestAsync<T>(string requestRoute, T requestBody)
        {
            if (!IsAuthenticated())
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "error: client was not authorized"
                };
            }

            try
            {
                var content = GetJsonContent(requestBody);
                var response = await _client.PutAsync(requestRoute, content);
                return await GetResponseContentAsync<ApiResponse>(response);
            }
            catch (ServerErrorException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Server Internal Error: {e}",
                    Data = e
                };
            }
            catch (HttpRequestException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Maybe currently API server is not available.",
                    Data = e
                };
            }
            catch (Exception e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = e.Message,
                    Data = e
                };
            }
        }

        protected async Task<ApiResponse> DeleteRequestAsync(string requestRoute)
        {
            if (!IsAuthenticated())
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "error: client was not authorized"
                };
            }

            try
            {
                var response = await _client.DeleteAsync(requestRoute);
                return await GetResponseContentAsync<ApiResponse>(response);
            }
            catch (ServerErrorException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = $"Server Internal Error: {e}",
                    Data = e
                };
            }
            catch (HttpRequestException e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = "Maybe currently API server is not available.",
                    Data = e
                };
            }
            catch (Exception e)
            {
                return new ApiResponse()
                {
                    Status = ApiResponseStatus.Error,
                    Message = e.Message,
                    Data = e
                };
            }
        }

        protected async Task<T> GetResponseContentAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseData, _serializerOptions);
            }
            catch (JsonException)
            {
                throw new ServerErrorException(response.ToString());
            }
        }

        protected StringContent GetJsonContent<T>(T data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            return new StringContent(jsonData, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}