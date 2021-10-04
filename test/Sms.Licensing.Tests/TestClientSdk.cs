using Sms.Licensing.Sdk.Client;
using Sms.Licensing.Shared;
using Sms.Licensing.Shared.Models;
using Xunit;

namespace Sms.Licensing.Tests
{
    public class TestClientSdk
    {
        private readonly SmsApiClientOptions _apiClientOptions;
        private readonly UserAdvancedCredentials _userCredentials;

        public TestClientSdk()
        {
            _apiClientOptions = new SmsApiClientOptions()
            {
                ApiServerAddress = "https://localhost:5001"
                //ServerAddress = "https://api.smartmealservice.com"
            };

            _userCredentials = new UserAdvancedCredentials()
            {
                UserName = "TestClientKiosk",
                Password = "Password1"
            };
        }

        [Fact]
        public async void TestAuthorization()
        {
            var client = new SmsApiClient(_apiClientOptions);
            var accessToken = await client.AuthenticatePasswordAsync(_userCredentials);
            Assert.NotNull(accessToken);
        }

        [Fact]
        public async void TestCheckLicense()
        {
            var client = new SmsApiClient(_apiClientOptions);
            await client.AuthenticatePasswordAsync(_userCredentials);
            var checkLicenseApiResponse = await client.CheckLicenseAsync();
            Assert.True(checkLicenseApiResponse.Status == ApiResponseStatus.Success);
            Assert.True(checkLicenseApiResponse.Data == LicenseStatus.Valid);
        }
    }
}
