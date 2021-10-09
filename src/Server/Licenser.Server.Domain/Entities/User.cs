using System;
using System.Collections.Generic;
using Licenser.Server.Domain.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Licenser.Server.Domain.Entities
{
    public class User : IdentityUser, IEntity<string>
    {
        public override string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public virtual IList<License> UserLicenses { get; set; } = new List<License>();

        public override string ToString()
        {
            return UserName;
        }
    }
}
