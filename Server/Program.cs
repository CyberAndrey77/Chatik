using System;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using Server.Repository;

namespace Server
{
    class Program
    {
        static void Main()
        {
            var fileManager =  new FileManager();
            var config = fileManager.GetConfig();
            //fileManager = null;
            //DataBaseManager dataBaseManager = new DataBaseManager(new Network(config.Port), new ChatRepositoryNoRight(config.ConnectionString));
            var dataBaseManager = new DataBaseManager(new Network(), config.ConnectionString)
            {
                Network =
                {
                    MessageHandlerDelegate = ShowMessage
                }
            };
            dataBaseManager.Network.StartSever(config.WaitTimeInSecond, config.Port);
            Console.ReadKey();
            dataBaseManager.Network.StopServer();
        }

        private static void ShowMessage(string message)
        {
            Console.Write(message);
        }
    }
}