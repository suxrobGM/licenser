namespace Sms.Licensing.Sdk.Client
{
    public class SmsApiClientOptions
    {
        public string ApiServerAddress { get; set; }
        public string IdentityServerAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }

        /// <summary>
        /// Use only in client apps.
        /// </summary>
        public string ProductName { get; set; }
    }
}