using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Licenser.Domain.Services.Abstractions;
using Licenser.Domain.Options;

namespace Licenser.Infrastructure.Services
{
    public class GoogleCaptchaService : IGoogleCaptchaService
    {
        private readonly GoogleCaptchaOptions _options;

        public GoogleCaptchaService(GoogleCaptchaOptions options)
        {
            _options = options;
        }

        public async Task<bool> CheckCaptchaResponseAsync(string responseToken)
        {
            var captchaApiUrl = _options.ApiUrl;
            var secretKey = _options.SecretKey;

            using var httpClient = new HttpClient();
            var postQueries = new List<KeyValuePair<string, string>>
            {
                new("secret", secretKey),
                new("response", responseToken)
            };

            var response = await httpClient.PostAsync(new Uri(captchaApiUrl), new FormUrlEncodedContent(postQueries));
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonData = JObject.Parse(responseContent);

            return jsonData["success"] != null && bool.Parse(jsonData["success"].ToString());
        }
    }
}