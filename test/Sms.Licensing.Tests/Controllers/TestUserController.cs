using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using Sms.Licensing.Shared.Models;

namespace Sms.Licensing.Tests
{
    public class TestUserController : TestAuthenticationController
    {
        [Fact]
        public async void TestUserRegister()
        {
            var user = new UserDto()
            {
                UserName = "TestClient",
                Email = "TestEmail@mail.ru",
                Password = "Password123"
            };

            var jsonData = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("user", content);

            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseData, serializerOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);
        }

        [Fact]
        public async void TestAddClient()
        {
            if (base.client.DefaultRequestHeaders.Authorization == null)
            {
                await AuthorizeAsAdmin(); // get access token before requesting
            }

            var client = new UserDto()
            {
                UserName = "TestClient"
            };

            var clientJsonData = JsonSerializer.Serialize(client);
            var content = new StringContent(clientJsonData, Encoding.UTF8, "application/json");
            var response = await base.client.PostAsync("user/add-client", content);
            
            var responseData = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseData, serializerOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Status == ApiResponseStatus.Success);
        }
    }
}
