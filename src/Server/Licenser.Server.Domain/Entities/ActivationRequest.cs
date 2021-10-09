using System;
using Licenser.Server.Domain.Entities.Abstractions;

namespace Licenser.Server.Domain.Entities
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
        public virtual User RequestedClient { get; set; }
    }
}