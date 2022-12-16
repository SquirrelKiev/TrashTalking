﻿using System.Net;
using System.Net.Sockets;

namespace TrashTalking
{
    // TODO: Better error handling for async funcs
    internal class Server
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid arguments.");

                return;
            }

            if (args[0] == "client")
            {
                await StartClient();

                return;
            }
            else if (args[0] == "server")
            {
                await StartServer();

                return;
            }
            else
            {
                Console.WriteLine("Invalid arguments.");

                return;
            }
        }

        private static async Task StartClient()
        {
            var chatClient = new ChatClient();

            await chatClient.ConnectToServer(IPAddress.Loopback);

            while(chatClient.Connected)
            {
                await chatClient.HandleUserInput();
            }
        }

        private static async Task StartServer()
        {
            var chatServer = new ChatServer(52014, IPAddress.Any);

            while (true)
            {
                await chatServer.AcceptConnections();
            }
        }
    }
}
