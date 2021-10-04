using System;
using System.IO;
using Sms.Licensing.Sdk.Client;

namespace Sms.Licensing.Server.Cli
{
    public class Program
    {
        private const string SecretKeyFile = "secret.txt";
        private const string MachineIdFile = "activation_id.txt";

        public static void Main(string[] args)
        {
            RunDeveloperMode();
            Console.WriteLine("\nPress Enter to exit.");
            Console.ReadLine();
        }

        private static void RunDeveloperMode()
        {
            Console.WriteLine("SMS Licensing CLI (Developer Mode)");

            while (true)
            {
                Console.WriteLine("\nChoose operation: ");
                Console.WriteLine("1. Generate random secret key for JWT authentication.");
                Console.WriteLine("2. Get unique ID for current machine.");
                Console.WriteLine("0. Exit from console.");
                Console.WriteLine("\nEnter 1, 2 or 0");
                
                var input = Console.ReadLine();
                if (input == "1")
                {
                    var secretKey = KeyGenerator.GenerateGuid();
                    var secretKeyHash = KeyGenerator.GetSecretKey(secretKey);
                    Console.WriteLine($"\nGenerated secret key (raw): {secretKey}");
                    Console.WriteLine($"\nGenerated secret key (SHA256 hash): {secretKeyHash}");
                    Console.WriteLine($"Key saved in the file {SecretKeyFile}");

                    if (!File.Exists(SecretKeyFile))
                    {
                        File.Create(SecretKeyFile).Close();
                    }

                    File.WriteAllText(SecretKeyFile, $"{secretKey}\n{secretKeyHash}");
                }
                else if (input == "2")
                {
                    var machineId = KeyGenerator.GetMachineId();
                    Console.WriteLine($"\nUnique machine ID: {machineId}");
                    Console.WriteLine($"Machine ID saved in the file {MachineIdFile}");

                    if (!File.Exists(MachineIdFile))
                    {
                        File.Create(MachineIdFile).Close();
                    }

                    File.WriteAllText(MachineIdFile, machineId);
                }
                else if (input == "0")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Wrong input please try again.");
                }
            }
        }
    }
}
