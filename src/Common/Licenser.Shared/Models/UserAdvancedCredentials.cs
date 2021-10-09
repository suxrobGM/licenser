using System;

namespace Licenser.Shared.Models
{
    [Serializable]
    public class UserAdvancedCredentials : UserBasicCredentials
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}