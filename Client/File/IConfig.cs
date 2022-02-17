namespace Client.File
{
    public interface IConfig
    {
        public int Port { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string PathToTheme { get; set; }

        void SaveSelf();
    }
}