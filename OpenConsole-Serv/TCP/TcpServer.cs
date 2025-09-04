using OpenConsole.Helpers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenConsole.TCP
{
    internal class TcpServer
    {
        public static void Start(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            CLog.Log("Server",$"Server started on port {port}...");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                CLog.Log("Server", $"Client connected: {client.Client.RemoteEndPoint}");
                Thread t = new Thread(HandleClient);
                t.Start(client);
            }
        }
        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message;
                    try
                    {
                        message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                    catch (Exception ex)
                    {
                        CLog.Log("Server", $"[Client {client.Client.RemoteEndPoint}] Invalid UTF-8 data: {ex.Message}");
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        CLog.Log("Server", $"[Client {client.Client.RemoteEndPoint}] Sent empty or whitespace message. Ignored.");
                        continue;
                    }
                    if (message.Length > 1024)
                    {
                        CLog.Log("Server", $"[Client {client.Client.RemoteEndPoint}] Message too long. Truncated.");
                        message = message.Substring(0, 1024);
                    }
                    string moduleName = "Server";
                    string logMessage = message;
                    int separatorIndex = message.IndexOf(" - ");
                    if (separatorIndex > 0)
                    {
                        moduleName = message.Substring(0, separatorIndex).Trim();
                        logMessage = message.Substring(separatorIndex + 3).Trim();
                    }
                    CLog.Log(moduleName, logMessage);
                }
            }
            catch (Exception ex)
            {
                CLog.Log("Server", $"[Client {client.Client.RemoteEndPoint}] Connection error: {ex.Message}");
            }
            finally
            {
                CLog.Log("Server", $"Client disconnected: {client.Client.RemoteEndPoint}");
                client.Close();
            }
        }
    }
}
