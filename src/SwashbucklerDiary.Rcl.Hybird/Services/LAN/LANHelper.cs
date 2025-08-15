using System.Net;
using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Services
{
    public static class LANHelper
    {
        public static string GetLocalIPv4Address()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 53);

            if (socket.LocalEndPoint is not IPEndPoint endPoint)
            {
                throw new InvalidOperationException("Failed to retrieve local IPv4 endpoint.");
            }

            return endPoint.Address.ToString();
        }
    }
}
