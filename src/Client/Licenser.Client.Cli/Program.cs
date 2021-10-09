using System;
using System.IO;
using Licenser.Sdk.Client;

namespace Licenser.Client.Cli
{
    public class Program
    {
        private const string MachineIdFile = "activation_id.txt";

        public static void Main(string[] args)
        {
            RunClientMode();
            Console.WriteLine("\nPress Enter to exit.");
            Console.ReadLine();
        }

        private static void RunClientMode()
        {
            Console.WriteLine("SMS Licensing CLI (Client Mode)");
            
            var machineId = KeyGenerator.GetMachineId();
            Console.WriteLine($"\nYour activation ID: {machineId}");
            Console.WriteLine($"Activation ID saved in the file {MachineIdFile}");

            if (!File.Exists(MachineIdFile))
            {
                File.Create(MachineIdFile).Close();
            }

            File.WriteAllText(MachineIdFile, machineId);
        }
    }
}
