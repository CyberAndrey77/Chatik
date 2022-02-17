using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.File
{
    public class FileManager: IFileManager
    {
        private readonly string _roamingDirectory;
        private readonly string _filePath;
        private readonly string _fileName;

        public FileManager()
        {
            _roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _filePath = Path.Combine(_roamingDirectory, "ClientConfig");
            _fileName = @"config.json";
        }

        public void SaveConfig(IConfig config)
        {
            System.IO.File.WriteAllText(Path.Combine(_filePath, _fileName), JsonConvert.SerializeObject(config));
        }

        public void LoadConfig(out IConfig config)
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            if (!System.IO.File.Exists(Path.Combine(_filePath, _fileName)))
            {
                config = new Config(this)
                {
                    Port = 8080,
                    IpAddress = string.Empty,
                    Name = string.Empty,
                    PathToTheme = @"../Themes/LightTheme.xaml"
                };
                SaveConfig(config);
            }
            config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(Path.Combine(_filePath, _fileName)));
        }
    }
}
