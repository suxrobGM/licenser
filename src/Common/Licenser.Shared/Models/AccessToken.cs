using System;

namespace Licenser.Shared.Models
{
    public class AccessToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}