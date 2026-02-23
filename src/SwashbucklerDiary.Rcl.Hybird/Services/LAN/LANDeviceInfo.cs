using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LANDeviceInfo
    {
        public string? DeviceName { get; set; }

        public string? IPAddress { get; set; }

        public AppPlatform DevicePlatform { get; set; }

        public DateTime LastSeen { get; set; }
    }
}
