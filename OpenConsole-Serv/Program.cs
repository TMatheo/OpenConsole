using System;
using OpenConsole.Helpers;
using OpenConsole.TCP;

namespace OpenConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleMisc.Setup();
            int port;
            if (args.Length == 0)
            {
                port = 50000;
                CLog.Log("Info", $"No port argument provided, Using default port {port}.");
            }
            else if (!int.TryParse(args[0], out port) || port < 1 || port > 65535)
            {
                Environment.Exit(0);
                return;
            }
            TcpServer.Start(port);
        }
    }
}
