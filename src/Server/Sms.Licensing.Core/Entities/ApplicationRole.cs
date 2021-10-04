using System;
using Microsoft.AspNetCore.Identity;
using Sms.Licensing.Core.Entities.Abstractions;

namespace Sms.Licensing.Core.Entities
{
    public class ApplicationRole : IdentityRole, IEntity<string>
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        public override string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return Name;
        }
    }
}