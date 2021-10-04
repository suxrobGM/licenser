using System;

namespace Sms.Licensing.Shared.Models
{
    [Serializable]
    public class UserAdvancedCredentials : UserBasicCredentials
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}