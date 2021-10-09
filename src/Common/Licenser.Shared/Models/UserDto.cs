using System;

namespace Licenser.Shared.Models
{
    public class UserDto : UserAdvancedCredentials
    {
        public string[] UserRoles { get; set; } = { "" };
        public DateTime RegistrationTime { get; set; } = DateTime.Now;
    }
}