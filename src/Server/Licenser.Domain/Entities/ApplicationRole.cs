using System;
using Licenser.Domain.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Licenser.Domain.Entities
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