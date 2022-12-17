namespace TrashTalking
{
    [Serializable]
    internal struct ClientReadyState
    {
        public ContentType contentType;

        public string username;

        public ClientReadyState(string username)
        {
            contentType = ContentType.ClientReady;

            this.username = username;
        }
    }
}