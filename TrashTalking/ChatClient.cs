using System.Net;
using System.Net.Sockets;

namespace TrashTalking
{
    /// <summary>
    /// Manages connecting and disconnecting to the server, and stuff relating to that. Kind of a wrapper for <see cref="ClientListener"/>.
    /// </summary>
    internal class ChatClient : IDisposable
    {
        public bool Connected => listener.Connected;

        ClientListener listener;

        public async Task ConnectToServer(IPAddress address, int port = 52014)
        {
            var client = new TcpClient();

            await client.ConnectAsync(address, port);

            listener = new ClientListener(client);

            listener.ClientDisconnected += Listener_ClientDisconnected;

            await listener.Initialize();
        }

        private void Listener_ClientDisconnected(DataListener listener)
        {
            listener.ClientDisconnected -= Listener_ClientDisconnected;

            listener.Dispose();
        }

        public async Task HandleUserInput()
        {
            var line = Console.ReadLine();
            
            if(Connected)
                await listener.SendMessage(new Message(listener.assignedId, line));
        }

        ~ChatClient()
        {
            Dispose();
        }

        public void Dispose()
        {
            listener.Dispose();
        }
    }
}
