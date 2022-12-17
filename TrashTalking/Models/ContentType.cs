using Newtonsoft.Json;

namespace TrashTalking
{
    // So the recieving end knows what is arriving
    // Format is Name of the sending tcp thing (client, server), and whatever the contents is. e.g. ClientSendMessage
    // TODO: better name
    public enum ContentType
    {
        BadData,
        ClientReady,
        ServerChatRoomState,
        ClientSendMessage,
    }

    [Serializable]
    internal struct EmptyContentType
    {
        public ContentType contentType;

        public EmptyContentType(ContentType contentType)
        {
            this.contentType = contentType;
        }
    }
}