using System.Net.Sockets;
using System.Text;

namespace TrashTalking
{
    enum DisconnectReason
    {
        Unknown,
        Left,
        TimedOut,
    }

    internal class ConnectedUser : IDisposable
    {
        public event Action<ConnectedUser, DisconnectReason> ClientDisconnected;

        public ChatServer ChatServer { get; }
        public TcpClient Client { get; }

        private Task checkForMessagesTask;

        private NetworkStream stream;
        private bool disposed = false;

        // Server
        public ConnectedUser(ChatServer chatServer, TcpClient clientRef) 
        {
            ChatServer = chatServer;
            Client = clientRef;

            clientRef.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            clientRef.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 2);
            clientRef.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
            clientRef.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3);

            checkForMessagesTask = CheckForMessages(clientRef);
        }

        public void Dispose()
        {
            CheckDisposed();

            disposed = true;

            Client.Dispose();

            stream.Dispose();
        }

        public async Task SendMessage(ConnectedUser sender, string message)
        {
            CheckDisposed();

            await stream.WriteAsync(Encoding.ASCII.GetBytes(message));
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(ConnectedUser));
        }

        private async Task CheckForMessages(TcpClient client)
        {
            CheckDisposed();

            DisconnectReason disconnectReason = DisconnectReason.Unknown;

            try
            {
                byte[] bytes = new byte[1024];

                stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = await stream.ReadAsync(bytes)) != 0 && !disposed)
                {
                    await ChatServer.SendMessageToAllClients(this, Encoding.ASCII.GetString(bytes));

                    Array.Clear(bytes);
                }

                disconnectReason = DisconnectReason.Left;
                Console.WriteLine("Client disconnected.");
            }
            catch(IOException)
            {
                disconnectReason = DisconnectReason.TimedOut;
                Console.WriteLine("Client timed out.");
            }
            catch(Exception)
            { }
            finally
            {
                ClientDisconnected.Invoke(this, disconnectReason);
            }

            return;
        }
    }
}
