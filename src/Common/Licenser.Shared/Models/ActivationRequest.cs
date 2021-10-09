using System;

namespace Licenser.Shared.Models
{
    public class ActivationRequest
    {
        public ActivationRequest()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
        }

        public string Id { get; set; }
        public string ActivationId { get; set; }
        public string RequestedClientId { get; set; }
        public string RequestedClientUserName { get; set; }
        public string ProductName { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum ActivationRequestStatus
    {
        RequestCreated,
        RequestAlreadyMade,
        AlreadyHaveValidLicense
    }
}