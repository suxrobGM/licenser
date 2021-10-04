using System;

namespace Sms.Licensing.Shared.Models
{
    [Serializable]
    public class UserBasicCredentials
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Password { get; set; }
    }
}