namespace TrashTalking
{
    [Serializable]
    internal struct ChatRoomState
    {
        public ContentType contentType;

        public List<ConnectedUserState> connectedUsers;

        public ChatRoomState(List<ConnectedUserState> connectedUsers)
        {
            contentType = ContentType.ServerChatRoomState;
            this.connectedUsers = connectedUsers;
        }

        public ChatRoomState(List<DataListener> connectedUsers)
        {
            var users = new List<ConnectedUserState>();

            foreach (var user in connectedUsers)
            {
                users.Add(new ConnectedUserState(user.assignedId, user.UserName));
            }

            contentType = ContentType.ServerChatRoomState;
            this.connectedUsers = users;
        }
    }

    [Serializable]
    internal struct ConnectedUserState
    {
        public int id;
        public string username;

        public ConnectedUserState(int id, string username)
        {
            this.id = id;
            this.username = username;
        }
    }
}
