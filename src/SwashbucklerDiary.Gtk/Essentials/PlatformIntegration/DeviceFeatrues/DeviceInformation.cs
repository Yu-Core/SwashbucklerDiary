using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public AppPlatform CurrentPlatform
            => OperatingSystem.IsLinux() ? AppPlatform.Linux : AppPlatform.Unknown;

        public string DeviceName => GLib.Functions.GetHostName();
    }
}
