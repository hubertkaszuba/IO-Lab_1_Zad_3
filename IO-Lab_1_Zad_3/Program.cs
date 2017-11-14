using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IO_Lab_1_Zad_3
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ThreadProc_server, new object[] { 1000 });
            ThreadPool.QueueUserWorkItem(ThreadProc_client, new object[] { 1 });
            ThreadPool.QueueUserWorkItem(ThreadProc_client, new object[] { 2 });

            Thread.Sleep(5000);
        }

        static void ThreadProc_client(Object stateInfo)
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));

            var nr = ((object[])stateInfo)[0];
            byte[] message = new ASCIIEncoding().GetBytes("Wiadomosc od watku nr: " + nr);
            byte[] buffer = new byte[1024];

            NetworkStream ns = client.GetStream();
            ns.Write(message, 0, message.Length);          
            ns.Read(buffer, 0, 1024);

            ConsoleColor color = ConsoleColor.Red;
            writeConsoleMessage(new ASCIIEncoding().GetString(buffer, 0, buffer.Length), color);
        }

        static void ThreadProc_server(Object stateInfo)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);

            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc_connection, client);

            }
        }
        static void ThreadProc_connection(object stateInfo)
        {
            byte[] buffer = new byte[1024];
            byte[] message = new ASCIIEncoding().GetBytes("Wiadomosc od serwera");

            var client = (TcpClient)stateInfo;
            client.GetStream().Read(buffer, 0, buffer.Length);
            client.GetStream().Write(message, 0, message.Length);
            ConsoleColor color = ConsoleColor.Green;
            writeConsoleMessage(new ASCIIEncoding().GetString(buffer, 0,buffer.Length), color);                     
            client.Close();
        }

        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }
}