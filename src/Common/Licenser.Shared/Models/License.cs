using System;

namespace Licenser.Shared.Models
{
    public class License
    {
        public License()
        {
            Id = Guid.NewGuid().ToString();
            IssueDate = DateTime.Now;
        }

        public string Id { get; set; }
        public string MachineId { get; set; }
        public string OwnerId { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerEmail { get; set; }
        public string ObjectName { get; set; }
        public string ProductName { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public enum LicenseStatus
    {
        Invalid,
        Valid,
        Expired
    }
}