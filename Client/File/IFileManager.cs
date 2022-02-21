namespace Client.File
{
    public interface IFileManager
    {
        void SaveConfig(IConfig config);
        void LoadConfig(out IConfig config);
    }
}