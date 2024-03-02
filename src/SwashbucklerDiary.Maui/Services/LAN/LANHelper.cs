using System.Net;
using System.Net.Sockets;

namespace SwashbucklerDiary.Maui.Services
{
    public static class LANHelper
    {
        public static string GetLocalIPv4Address()
        {
            using Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
            return ((IPEndPoint)s.LocalEndPoint!).Address.ToString();
        }

        public static string GetLocalDeviceName()
            => DeviceInfo.Current.Name;
    }
}
