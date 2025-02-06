using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public AppDevicePlatform CurrentPlatform
            => OperatingSystem.IsLinux() ? AppDevicePlatform.Linux : AppDevicePlatform.Unknown;

        public string DeviceName => GLib.Functions.GetUserName();
    }
}
