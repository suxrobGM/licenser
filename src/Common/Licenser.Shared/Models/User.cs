using System;

namespace Licenser.Shared.Models
{
    public class User : UserAdvancedCredentials
    {
        public string[] UserRoles { get; set; } = { "" };
        public DateTime RegistrationTime { get; set; } = DateTime.Now;
    }
}