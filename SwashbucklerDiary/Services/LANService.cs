using Microsoft.Maui.Devices;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SwashbucklerDiary.Services
{
    public class LANService : ILANService
    {
        private readonly Dictionary<DevicePlatformType, string> DeviceIcons = new()
        {
            {DevicePlatformType.Windows,"mdi-microsoft-windows" },
            {DevicePlatformType.Android,"mdi-android" },
            {DevicePlatformType.iOS,"mdi-apple-ios" },
            {DevicePlatformType.MacOS,"mdi-apple" },
            {DevicePlatformType.Unknown,"mdi-monitor-cellphone" },
        };

        public string GetIPPrefix(string ipAddress)
        {
            string[] ipParts = ipAddress.Split('.');

            StringBuilder ipPrefix = new StringBuilder();

            for (int i = 0; i < ipParts.Length - 1; i++)
            {
                ipPrefix.Append(ipParts[i]);
                ipPrefix.Append('.');
            }

            return ipPrefix.ToString();
        }

        public string GetLocalIPv4()
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
                string ip = ((IPEndPoint)s.LocalEndPoint).Address.ToString();
                s.Close();
                return ip;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        public bool Ping(IPAddress address)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(address, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public LANDeviceInfo GetLocalLANDeviceInfo()
        {
            string deviceName = GetLocalDeviceName();
            string ipAddress = GetLocalIPv4();
            DevicePlatformType platformType = GetLocalDevicePlatformType();

            return new()
            {
                DeviceName = deviceName,
                IPAddress = ipAddress,
                DevicePlatform = platformType
            };
        }

        public string GetLocalDeviceName()
        {
            return DeviceInfo.Current.Name;
        }

        public DevicePlatformType GetLocalDevicePlatformType()
        {
            if(OperatingSystem.IsWindows())
            {
                return DevicePlatformType.Windows;
            }

            if(OperatingSystem.IsLinux())
            {
                return DevicePlatformType.Linux;
            }

            if(OperatingSystem.IsAndroid())
            {
                return DevicePlatformType.Android;
            }

            if (OperatingSystem.IsIOS())
            {
                return DevicePlatformType.iOS;
            }

            if (OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS())
            {
                return DevicePlatformType.MacOS;
            }

            if (OperatingSystem.IsBrowser())
            {
                return DevicePlatformType.Android;
            }

            return DevicePlatformType.Unknown;
        }

        public string GetDevicePlatformTypeIcon(DevicePlatformType platformType)
        {
            if (DeviceIcons.ContainsKey(platformType))
            {
                return DeviceIcons[platformType];
            }
            else
            {
                return DeviceIcons[DevicePlatformType.Unknown];
            }
        }

        public bool IsConnection()
        {
            Microsoft.Maui.Networking.NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == Microsoft.Maui.Networking.NetworkAccess.None)
            {
                return false;
            }

            return true;
        }
    }
}
