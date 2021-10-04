using System;

namespace Sms.Licensing.Shared.Models
{
    public class AccessToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}