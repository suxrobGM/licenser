using System;
using Sms.Licensing.Core.Entities.Abstractions;

namespace Sms.Licensing.Core.Entities
{
    public class ActivationRequest : IEntity<string>
    {
        public ActivationRequest()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
        }

        public string Id { get; set; }
        public string ActivationId { get; set; }
        public string ProductName { get; set; }
        public DateTime Timestamp { get; set; }

        public string RequestedClientId { get; set; }
        public virtual ApplicationUser RequestedClient { get; set; }
    }
}