namespace TrashTalking
{
    [Serializable]
    internal struct Message
    {
        public ContentType contentType;

        public int senderIndex;

        public string message;

        public Message(int senderIndex, string message)
        {
            contentType = ContentType.ClientSendMessage;
            this.senderIndex = senderIndex;
            this.message = message;
        }
    }
}