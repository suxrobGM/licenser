using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Sms.Licensing.Domain.Entities.Abstractions;

namespace Sms.Licensing.Domain.Entities
{
    public class ApplicationUser : IdentityUser, IEntity<string>
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
