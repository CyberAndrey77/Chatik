using System;
using System.Net.Mime;
using System.Runtime.CompilerServices;

namespace Server
{
    class Program
    {
        static void Main()
        {
            var config = FileManager.GetConfig();
            Network network = new Network(config);
            network.StartSever(ShowMessage);
            Console.ReadKey();
            network.StopServer();
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}