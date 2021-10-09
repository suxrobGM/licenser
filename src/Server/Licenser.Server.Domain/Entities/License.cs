using System;
using Licenser.Server.Domain.Entities.Abstractions;

namespace Licenser.Server.Domain.Entities
{
    public class License : IEntity<string>
    {
        public License()
        {
            Id = Guid.NewGuid().ToString();
            IssueDate = DateTime.Now;
        }

        public string Id { get; set; }
        public string MachineId { get; set; }
        public string OwnerId { get; set; }
        public string ObjectName { get; set; }
        public string ProductName { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}