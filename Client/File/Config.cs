using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity;
using Unity.Injection;

namespace Client.File
{
    public class Config : IConfig
    {
        public int Port { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string PathToTheme { get; set; }
        private readonly IFileManager _fileManager;
        
        [InjectionConstructor]
        public Config(IFileManager fileManager)
        {
            _fileManager = fileManager;
            _fileManager.LoadConfig(out var config);
            Name = config.Name;
            IpAddress = config.IpAddress;
            Port = config.Port;
            PathToTheme = config.PathToTheme;
        }

        public void SaveSelf()
        {
           _fileManager.SaveConfig(this);
        }

        [JsonConstructor]
        public Config() { }
    }
}
