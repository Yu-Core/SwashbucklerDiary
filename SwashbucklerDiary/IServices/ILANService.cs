using SwashbucklerDiary.Models;
using System.Net;

namespace SwashbucklerDiary.IServices
{
    public interface ILANService
    {
        public bool IsConnection();
        public string GetLocalIPv4();
        public string GetIPPrefix(string ipAddress);
        public bool Ping(IPAddress address);
        public LANDeviceInfo GetLocalLANDeviceInfo();
        public string GetLocalDeviceName();
        public DevicePlatformType GetLocalDevicePlatformType();
        public string GetDevicePlatformTypeIcon(DevicePlatformType platformType);
    }
}
