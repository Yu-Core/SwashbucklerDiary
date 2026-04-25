using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public AppPlatform CurrentPlatform { get; }
            = OperatingSystem.IsLinux() ? new LinuxAppPlatform() : new UnknownAppPlatform();

        public string DeviceName => GLib.Functions.GetHostName();
    }
}
