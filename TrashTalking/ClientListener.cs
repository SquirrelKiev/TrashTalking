using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace TrashTalking
{
    internal class ClientListener : DataListener
    {
        ConnectedUserState[] connectedUsers;

        public ClientListener(TcpClient client) : base(client)
        {

        }

        internal override async Task Initialize()
        {
            string handshakeJson = JsonConvert.SerializeObject(new ClientReadyState($"farts {Random.Shared.Next(1,1000)}"));
            byte[] data = Encoding.UTF8.GetBytes(handshakeJson);

            await stream.WriteAsync(data);
        }

        protected override async Task RecievedData(ContentType contentType, string json)
        {
            switch(contentType)
            {
                case ContentType.ServerChatRoomState:
                    OnRecievedChatRoomState(JsonConvert.DeserializeObject<ChatRoomState>(json));
                    break;

                case ContentType.ClientSendMessage:
                    OnRecievedMessage(json);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void OnRecievedMessage(string json)
        {
            var messageObj = JsonConvert.DeserializeObject<Message>(json);

            Console.WriteLine($"{connectedUsers.First(x => { return x.id == messageObj.senderIndex; }).username}: {messageObj.message}");
        }

        private void OnRecievedChatRoomState(ChatRoomState state)
        {
            connectedUsers = state.connectedUsers.ToArray();

            foreach(var user in connectedUsers)
            {
                Console.WriteLine(user.username);
            }
        }
    }
}
