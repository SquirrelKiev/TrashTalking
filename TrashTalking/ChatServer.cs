using System.Net.Sockets;
using System.Net;

namespace TrashTalking
{
    /// <summary>
    /// Manages chat server connections.
    /// </summary>
    internal class ChatServer : IDisposable
    {
        internal int currentUserId { get; private set; }

        public List<ServerListener> Clients { get; } = new List<ServerListener>();

        private TcpListener listener;

        public ChatServer(int port, IPAddress addr)
        {
            listener = new(addr, port);

            listener.Start();
        }

        public async Task AcceptConnections()
        {
            Console.WriteLine($"Waiting for any new connections... ");
            TcpClient client = await listener.AcceptTcpClientAsync();

            IPEndPoint ipEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
            Console.WriteLine($"Connection recieved from {ipEndpoint.Address}!");

            var user = new ServerListener(client, this);

            user.ClientDisconnected += ClientDisconnected;
            user.OnUserReady += SendRoomState;

            await user.Initialize();

            Clients.Add(user);

            currentUserId++;
        }

        private void SendRoomState()
        {
            foreach (var client in Clients)
            {
                // need to make this await
                client.SendRoomState();
            }
        }

        internal ChatRoomState GetChatRoomState()
        {
            return new ChatRoomState(Clients.Select(x => (DataListener)x).ToList());
        }

        private void ClientDisconnected(DataListener user)
        {
            var client = (ServerListener)user;

            client.ClientDisconnected -= ClientDisconnected;

            client.Dispose();

            Clients.Remove(client);

            SendRoomState();
        }

        public async Task SendMessageToAllClients(Message message)
        {
            

            foreach (var user in Clients)
            {
                Console.WriteLine($"{message.senderIndex} {user.assignedId}");

                if (message.senderIndex == user.assignedId)
                    continue;

                await user.SendMessage(message);
            }
        }

        public void Dispose()
        {
            foreach(var client in Clients)
            {
                client.Dispose();
            }
        }
    }
}
