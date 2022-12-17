using Newtonsoft.Json;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;

namespace TrashTalking
{
    internal class ServerListener : DataListener
    {
        private ChatServer listener;

        public event Action OnUserReady;
        public bool userIsReady = false;

        public ServerListener(TcpClient client, ChatServer listener) : base(client)
        {
            this.listener = listener;
        }

        internal override async Task Initialize()
        {
            assignedId = listener.currentUserId;
        }

        protected override async Task RecievedData(ContentType contentType, string json)
        {
            try
            {
                switch (contentType)
                {
                    case ContentType.ClientReady:
                        OnClientReady(json);
                        break;

                    case ContentType.ClientSendMessage:
                        await OnMessageRecieved(json);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task OnMessageRecieved(string json)
        {
            var messageObj = JsonConvert.DeserializeObject<Message>(json);

            messageObj.senderIndex = assignedId;

            await listener.SendMessageToAllClients(messageObj);
        }

        private void OnClientReady(string clientReadyJson)
        {
            var clientInfo = JsonConvert.DeserializeObject<ClientReadyState>(clientReadyJson);

            UserName = clientInfo.username;

            userIsReady = true;
            OnUserReady.Invoke();
        }

        public async Task SendRoomState()
        {
            var chatRoomStateJson = JsonConvert.SerializeObject(listener.GetChatRoomState());

            var data = Encoding.UTF8.GetBytes(chatRoomStateJson);

            await stream.WriteAsync(data);
        }
    }
}
