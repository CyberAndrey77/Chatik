using Newtonsoft.Json;
using System;
using System.IO;

namespace Server
{
    public class FileManager
    {
        private readonly string _roamingDirectory;
        private readonly string _filePath;
        private readonly string _fileName;

        public FileManager()
        {
            _roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            _filePath = Path.Combine(_roamingDirectory, "ServerConfig");
            _fileName = @"config.json";
        }

        public Config GetConfig()
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            if (!File.Exists(Path.Combine(_filePath, _fileName)))
            {
                CreateConfig(600000, 8080, @"Server=(localdb)\mssqllocaldb; Database=ChatDatabase; Trusted_Connection=True;");
            }
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(_filePath, _fileName)));
            return config;
        }

        public void CreateConfig(int time, int port, string stringConnection)
        {
            var config = new Config()
            {
                WaitTimeInSecond = time,
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

        public void CreateLog(string message)
        {
            string filePath = Path.Combine(_filePath, "log.txt");
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message);
            }
        }
    }
}
