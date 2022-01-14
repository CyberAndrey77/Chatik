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
            var config = FileManager.GetConfig();
            //DataBaseManager dataBaseManager = new DataBaseManager(new Network(config.Port), new ChatRepository(config.ConnectionString));
            DataBaseManager dataBaseManager = new DataBaseManager(new Network(config.Port), config.ConnectionString);
            dataBaseManager.Network.StartSever(ShowMessage);
            Console.ReadKey();
            dataBaseManager.Network.StopServer();
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}