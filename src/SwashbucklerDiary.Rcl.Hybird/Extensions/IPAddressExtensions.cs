using System.Net;
using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Hybird
{
    public static class IPAddressExtensions
    {
        public static int GetAvailablePort(this IPAddress ip)
        {
            using var listener = new TcpListener(ip, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
