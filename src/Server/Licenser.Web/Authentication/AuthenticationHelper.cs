using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Licenser.Sdk.Server;

namespace Licenser.Web.Authentication
{
    public class AuthenticationHelper
    {
        private readonly ISmsApiServer _apiServer;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationHelper(ISmsApiServer apiServer,
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _apiServer = apiServer;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        /// <summary>
        /// Checks user authentication status. User should have admin access in order to authorize successfully.
        /// </summary>
        /// <param name="redirectToLoginPage">Redirect to login page if unsuccessful authorization</param>
        /// <returns>True if user authorized successfully</returns>
        public async Task<bool> CheckAuthenticationAsync(bool redirectToLoginPage = true)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var isAuthenticated = authState.User.Identity?.IsAuthenticated;
            var isAuthenticatedInWeb = isAuthenticated.HasValue && isAuthenticated.Value;

            switch (isAuthenticatedInWeb)
            {
                case true when !_apiServer.IsAuthenticated():
                    _navigationManager.NavigateTo("/SigninOidc", true);
                    break;
                case false when redirectToLoginPage:
                    _navigationManager.NavigateTo("/Login", true);
                    break;
            }

            return isAuthenticatedInWeb;
        }
    }
}