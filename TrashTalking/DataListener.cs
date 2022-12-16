using System.Net.Sockets;
using System.Text;

namespace TrashTalking
{
    internal abstract class DataListener : IDisposable
    {

        public event Action<DataListener> ClientDisconnected;

        public bool Connected => !disposed;
        public string UserName { get; protected set; }
        public ChatServer Listener { get; }
        public TcpClient Client { get; }

        private Task checkForMessagesTask;

        protected NetworkStream stream;
        private bool disposed = false;

        public DataListener(TcpClient client, ChatServer listener = null)
        {
            Client = client;
            Listener = listener;

            if(Listener != null)
            {
                UserName = "Server";
            }

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 2);
            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3);

            checkForMessagesTask = Listen(client);
        }

        public void Dispose()
        {
            CheckDisposed();

            disposed = true;

            Client.Dispose();
        }

        public async Task SendMessage(DataListener sender, string message)
        {
            CheckDisposed();

            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataListener));
        }

        private async Task Listen(TcpClient client)
        {
            CheckDisposed();

            try
            {
                byte[] bytes = new byte[1024];

                stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = await stream.ReadAsync(bytes)) != 0 && !disposed)
                {
                    await RecievedData(bytes);

                    Array.Clear(bytes);
                }

                Console.WriteLine("Client disconnected. (end of stream)");
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex);

                Console.WriteLine("Client disconnected. (IOException)");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                ClientDisconnected.Invoke(this);
            }

            return;
        }

        public abstract Task RecievedData(byte[] bytes);

        internal virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
