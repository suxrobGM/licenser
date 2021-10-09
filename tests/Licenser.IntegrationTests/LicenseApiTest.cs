using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using Licenser.Shared.Models;

namespace Licenser.IntegrationTests
{
    public class TestLicenseApi : TestAuthentication
    {
        [Fact]
        public async void TestAddLicense()
        {
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                await AuthorizeAsAdmin(); // get access token before requesting
            }

            var license = new License()
            {
                OwnerId = "d5bbb4da-c304-4910-b237-d36b46803af5", // TestClient's ID
                MachineId = "PGPTV028J85093.BFEBFBFF000906E9",
            };

            var jsonData = JsonSerializer.Serialize(license);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("license", content);
            
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseData, serializerOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);
        }
        
        [Fact]
        public async void TestCheckFakeLicense()
        {
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                await AuthorizeAsAdmin(); // get access token before requesting
            }

            var license = new License()
            {
                OwnerId = "d5bbb4da-c304-4910-b237-d36b46803af5", // TestClient's ID
                MachineId = "PGPTV028J85093.BFEBFBFF000906E9"
            };

            var jsonData = JsonSerializer.Serialize(license);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("license/check", content);
            
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LicenseStatus>>(responseData, serializerOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);

            var licenseStatus = apiResponse.Data;
            Assert.True(licenseStatus == LicenseStatus.Invalid);
        }

        [Fact]
        public async void TestGetAllLicenses()
        {
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                await AuthorizeAsAdmin(); // get access token before requesting
            }

            var response = await client.GetAsync("license");
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<License>>>(responseData, serializerOptions);
            
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);
        }
    }
}
