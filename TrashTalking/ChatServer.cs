using System.Net.Sockets;
using System.Net;

namespace TrashTalking
{
    internal class ChatServer : IDisposable
    {
        public List<DataListener> Clients { get; } = new List<DataListener>();

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

            var user = new ConnectedUserServer(client, this);
            user.ClientDisconnected += ClientDisconnected;

            await user.Initialize();

            Clients.Add(user);
        }

        private void ClientDisconnected(DataListener user)
        {
            user.ClientDisconnected -= ClientDisconnected;

            user.Dispose();

            Clients.Remove(user);

        }

        public async Task SendMessageToAllClients(DataListener sender, string message)
        {
            foreach (var user in Clients)
            {
                if (user == sender)
                    continue;

                await user.SendMessage(sender, message);
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
