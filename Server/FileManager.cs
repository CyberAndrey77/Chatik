using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    public static class FileManager
    {
        private static readonly string _roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string _filePath = Path.Combine(_roamingDirectory, "ServerConfig");
        private static readonly string _fileName = @"config.json";

        public static Config GetConfig()
        {
            if (!File.Exists(Path.Combine(_filePath, _fileName)))
            {
                CreateConfig(8080, @"Server=(localdb)\mssqllocaldb; Database=ChatDatabase; Trusted_Connection=True;");
            }

            string configString;
            using (StreamReader streamReader = new StreamReader(Path.Combine(_filePath, _fileName)))
            {
                configString = streamReader.ReadToEnd();
            }

            Config config = JsonConvert.DeserializeObject<Config>(configString);
            return config;
        }

        public static void CreateConfig(int port, string stringConnection)
        {
            var config = new Config()
            {
                Port = port,
                ConnectionString = stringConnection
            };
            string filePath = Path.Combine(_filePath, _fileName);
            string configString = JsonConvert.SerializeObject(config);
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(configString);
            }
        }
    }
}
