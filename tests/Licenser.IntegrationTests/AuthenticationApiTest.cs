using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Licenser.Shared.Models;

namespace Licenser.IntegrationTests
{
    public class AuthenticationApiTest
    {
        protected readonly HttpClient client;
        protected readonly JsonSerializerOptions serializerOptions;

        public AuthenticationApiTest()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:5001/v1/") 
            };

            serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task AuthorizeAsAdmin()
        {
            var admin = new User()
            {
                UserName = "SuxrobGM",
                Password = "Password123"
            };

            var adminJsonData = JsonSerializer.Serialize(admin);
            var content = new StringContent(adminJsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("auth/authorize", content);

            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccessToken>>(responseData, serializerOptions);
            
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);

            var accessToken = apiResponse.Data;
            Assert.True(!string.IsNullOrEmpty(accessToken.Token));

            // Use token for next testing methods
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
        }

        [Fact]
        public async void TestFakeAuthorization()
        {
            var fakeClient = new User()
            {
                UserName = "FakeClient",
                Password = "Password123"
            };
            
            var jsonData = JsonSerializer.Serialize(fakeClient);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("auth/authorize", content);

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}
