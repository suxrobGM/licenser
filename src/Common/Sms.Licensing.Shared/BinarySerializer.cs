using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sms.Licensing.Shared
{
    public static class BinarySerializer
    {
        public static void Serialize<TData>(TData Data, string outputFileName) where TData: class
        {
            var formatter = new BinaryFormatter();
            using var fileStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
            formatter.Serialize(fileStream, Data);
        }

        public static TData Deserialize<TData>(string inputFileName) where TData: class
        {
            if (!File.Exists(inputFileName))
                return null;
            try
            {
                var formatter = new BinaryFormatter();
                using var fileStream = new FileStream(inputFileName, FileMode.OpenOrCreate);
                var data = formatter.Deserialize(fileStream) as TData;
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}