using System;
using System.Security.Cryptography;
using System.Text;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

namespace Sms.Licensing.Sdk.Client
{
    /// <summary>
    /// Static class for generating unique keys.
    /// </summary>
    public static class KeyGenerator
    {
        /// <summary>
        /// Gets SHA256 hash string from input.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>SHA256 string</returns>
        public static string GetSecretKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generates GUID.
        /// </summary>
        /// <returns></returns>
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        /// <summary>
        /// Gets unique machine ID which contains with combination of processor and motherboard ID.
        /// </summary>
        /// <returns>Unique machine ID</returns>
        public static string GetMachineId()
        {
            var deviceId = new DeviceIdBuilder()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .UseFormatter(new HashDeviceIdFormatter(SHA256.Create, new Base64UrlByteArrayEncoder()));

            return deviceId.ToString();
        }
    }
}