using Newtonsoft.Json;

namespace TrashTalking
{
    // So the recieving end knows what is arriving
    // TODO: better name
    public enum State
    {
        ClientReady,
        ServerChatRoomState,
        BadData
    }

    [Serializable]
    internal struct SimpleState
    {
        public State state;

        public SimpleState(State state)
        {
            this.state = state;
        }
    }
}