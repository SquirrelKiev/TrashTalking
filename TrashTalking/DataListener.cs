using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace TrashTalking
{
    // i hate naming
    /// <summary>
    /// Individual client connected to the server. 
    /// Abstract so that the client and server can have their own ways of handling stuff. 
    /// </summary>
    internal abstract class DataListener : IDisposable
    {
        public event Action<DataListener> ClientDisconnected;

        public bool Connected => !disposed;
        public int assignedId { get; protected set; }
        public string UserName { get; protected set; }
        public ChatServer Listener { get; }
        /// <summary>
        /// The client the <see cref="DataListener"/> represents. 
        /// </summary>
        public TcpClient Client { get; }

        private Task checkForMessagesTask;

        protected NetworkStream stream;
        private bool disposed = false;

        public DataListener(TcpClient client)
        {
            Client = client;

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

        public async Task SendMessage(Message message)
        {
            CheckDisposed();

            var messageJson = JsonConvert.SerializeObject(message);

            Console.WriteLine(messageJson);

            await stream.WriteAsync(Encoding.UTF8.GetBytes(messageJson));
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
                    var json = Encoding.UTF8.GetString(bytes);

                    var contentType = ModelUtility.GetContentType(json);

                    Console.WriteLine(json);

                    await RecievedData(contentType, json);

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

        protected abstract Task RecievedData(ContentType contentType, string json);

        internal virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
