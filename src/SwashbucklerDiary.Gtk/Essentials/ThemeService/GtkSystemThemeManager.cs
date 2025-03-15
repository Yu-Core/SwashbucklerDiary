using Gio;
using GLib;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class GtkSystemThemeManager
    {
        private readonly global::Gtk.Settings _gtkSettings;

        public Theme SystemTheme { get; set; }

        public event Action<Theme>? SystemThemeChanged;

        public GtkSystemThemeManager()
        {
            _gtkSettings = global::Gtk.Settings.GetDefault()!;
            _gtkSettings.OnNotify += OnSystemThemeChanged;

            SystemTheme = GetCurrentSystemTheme();
        }

        private void OnSystemThemeChanged(GObject.Object sender, GObject.Object.NotifySignalArgs args)
        {
            var theme = GetCurrentSystemTheme();
            if (theme != SystemTheme)
            {
                SystemTheme = theme;
                SystemThemeChanged?.Invoke(SystemTheme);
            }
        }

        private Theme GetCurrentSystemTheme()
        {
            // from https://github.com/DevToys-app/DevToys/blob/main/src/app/dev/platforms/desktop/DevToys.Linux/Core/ThemeListener.cs
            try
            {
                var bus = DBusConnection.Get(BusType.Session);
                using var parameters = Variant.NewTuple([
                    Variant.NewString("org.freedesktop.appearance"), 
                    Variant.NewString("color-scheme")
                ]);

                using Variant ret = bus.CallSync(
                    busName: "org.freedesktop.portal.Desktop",
                    objectPath: "/org/freedesktop/portal/desktop",
                    interfaceName: "org.freedesktop.portal.Settings",
                    methodName: "Read",
                    parameters: parameters,
                    replyType: VariantType.New("(v)"),
                    flags: DBusCallFlags.None,
                    timeoutMsec: 2000,
                    cancellable: null
                );

                uint userThemePreference = ret.GetChildValue(0).GetVariant().GetVariant().GetUint32();
                return userThemePreference switch
                {
                    1 => Theme.Dark,
                    2 => Theme.Light,
                    _ => Theme.Light
                };
            }
            catch (Exception)
            {
                return _gtkSettings.GtkThemeName?.Contains("Dark", StringComparison.OrdinalIgnoreCase) ?? false
                ? Theme.Dark
                : Theme.Light;
            }
        }
    }
}
