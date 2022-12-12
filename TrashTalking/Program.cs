using System.Net;
using System.Net.Sockets;

namespace TrashTalking
{
    // TODO: Make not static
    // TODO: Better error handling for async funcs
    internal class Server
    {
        static async Task Main(string[] args)
        {
            var chatServer = new ChatServer(52014, IPAddress.Any);

            while (true)
            {
                await chatServer.AcceptConnections();
            }
        }
    }
}
