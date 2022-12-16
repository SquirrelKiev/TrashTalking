namespace TrashTalking
{
    [Serializable]
    internal struct ChatState
    {
        public State state;

        public List<ConnectedUserState> connectedUsers;
    }

    [Serializable]
    internal struct ConnectedUserState
    {
        public string username;
    }
}
