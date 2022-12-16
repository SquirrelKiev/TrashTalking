using System.Net.Sockets;
using System.Text;

namespace TrashTalking
{
    internal class ConnectedUserServer : DataListener
    {
        public ConnectedUserServer(TcpClient client, ChatServer listener = null) : base(client, listener)
        {
            
        }

        public override async Task RecievedData(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);

            var state = ModelUtility.GetState(json);

            Console.WriteLine(state);
            Console.WriteLine(json);
        }
    }
}
