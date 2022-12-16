using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace TrashTalking
{
    internal class ClientListener : DataListener
    {
        public ClientListener(TcpClient client, ChatServer listener = null) : base(client, listener)
        {

        }

        internal override async Task Initialize()
        {
            string handshakeJson = JsonConvert.SerializeObject(new SimpleState(State.ClientReady));
            byte[] data = Encoding.UTF8.GetBytes(handshakeJson);

            await stream.WriteAsync(data);
        }

        public override async Task RecievedData(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);

            var state = ModelUtility.GetState(json);

            Console.WriteLine(json);
        }
    }
}
