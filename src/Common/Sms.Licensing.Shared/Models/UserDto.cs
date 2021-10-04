using System;

namespace Sms.Licensing.Shared.Models
{
    public class UserDto : UserAdvancedCredentials
    {
        public string[] UserRoles { get; set; } = { "" };
        public DateTime RegistrationTime { get; set; } = DateTime.Now;
    }
}