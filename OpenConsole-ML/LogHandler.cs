using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using MelonLoader;

namespace OpenCommunicator
{
    internal class LogHandler
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static TcpClient tcpClient;
        private static StreamWriter tcpWriter;

        private static TextWriter originalConsoleOut;

        internal static void Setup()
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 50000);
                tcpWriter = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true };
                Console.WriteLine("Connected to TCP server 127.0.0.1:50000");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }

            originalConsoleOut = Console.Out;
            Console.SetOut(new InterceptWriter(originalConsoleOut));

            MelonLogger.MsgCallbackHandler += Message;
            MelonLogger.WarningCallbackHandler += Warning;
            MelonLogger.ErrorCallbackHandler += Error;
        }

        private static void SendToServer(string mod, string log)
        {
            try
            {
                if (tcpWriter != null)
                {
                    string msg = $"{mod} - {log}";
                    tcpWriter.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                originalConsoleOut?.WriteLine("TCP send failed: " + ex.Message);
            }
        }

        internal static void Warning(string mod, string log)
        {
            SendToServer(mod, log);
        }

        internal static void Error(string mod, string log)
        {
            SendToServer(mod, log);
        }

        internal static void Message(ConsoleColor color1, ConsoleColor color2, string arg3, string arg)
        {
            SendToServer($"<color={color1}>{arg3}", arg);
        }

        private class InterceptWriter : TextWriter
        {
            private readonly TextWriter original;

            public InterceptWriter(TextWriter original)
            {
                this.original = original;
            }

            public override Encoding Encoding => original.Encoding;

            public override void WriteLine(string value)
            {
                SendToServer("<color=#8742f5>Console", value);
            }

            public override void Write(char value)
            {
                original.Write(value);
            }

            public override void Write(string value)
            {
                original.Write(value);
            }
        }
    }
}
