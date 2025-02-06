using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Models
{
    public class LANDeviceInfo
    {
        public string? DeviceName { get; set; }

        public string? IPAddress { get; set; }

        public AppDevicePlatform DevicePlatform { get; set; }
    }
}
