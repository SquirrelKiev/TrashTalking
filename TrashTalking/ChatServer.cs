using System.Net.Sockets;
using System.Net;

namespace TrashTalking
{
    internal class ChatServer
    {
        public static List<ConnectedUser> Clients { get; } = new List<ConnectedUser>();

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

            var ipEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Connection recieved from {ipEndpoint.Address}!");

            var user = new ConnectedUser(this, client);
            user.ClientDisconnected += ClientDisconnected;

            Clients.Add(user);
        }

        private void ClientDisconnected(ConnectedUser user, DisconnectReason disconnectReason)
        {
            user.ClientDisconnected -= ClientDisconnected;

            user.Dispose();

            Clients.Remove(user);

        }

        public async Task SendMessageToAllClients(ConnectedUser sender, string message)
        {
            foreach (var user in Clients)
            {
                if (user == sender)
                    continue;

                await user.SendMessage(sender, message);
            }
        }
    }
}
