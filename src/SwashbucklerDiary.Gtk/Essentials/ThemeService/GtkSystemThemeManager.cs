using SwashbucklerDiary.Shared;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class GtkSystemThemeManager
    {
        public Theme SystemTheme { get; set; }

        public event Action<Theme>? SystemThemeChanged;

        public async Task InitializedAsync()
        {
            SystemTheme = await GetCurrentSystemThemeAsync();
            global::Gtk.Settings.Default.AddNotification(OnSystemThemeChanged);
        }

        private async void OnSystemThemeChanged(object o, GLib.NotifyArgs args)
        {
            var theme = await GetCurrentSystemThemeAsync();
            if (theme != SystemTheme)
            {
                SystemTheme = theme;
                SystemThemeChanged?.Invoke(SystemTheme);
            }
        }

        private static async Task<Theme> GetCurrentSystemThemeAsync()
        {
            try
            {
                Connection connection = new Connection(Address.Session);
                uint userThemePreference = await connection.CallMethodAsync(CreateMessage(), (m, s) =>
                {
                    var reader = m.GetBodyReader();
                    reader.ReadSignature("u"u8);
                    return reader.ReadUInt32();
                });
                MessageBuffer CreateMessage()
                {
                    var writer = connection.GetMessageWriter();
                    writer.WriteMethodCallHeader(
                        destination: "org.freedesktop.portal.Desktop",
                        path: "/org/freedesktop/portal/desktop",
                        @interface: "org.freedesktop.portal.Settings",
                        member: "Read",
                        signature: "ss",
                        flags: MessageFlags.None
                    );
                    writer.WriteString("org.freedesktop.appearance");
                    writer.WriteString("color-scheme");
                    return writer.CreateMessage();
                }

                return userThemePreference switch
                {
                    1 => Theme.Dark,
                    2 => Theme.Light,
                    _ => Theme.Light
                };
            }
            catch (Exception ex)
            {
                return FallBack();
            }

            Theme FallBack()
            {
                return global::Gtk.Settings.Default.ThemeName?.Contains("Dark", StringComparison.OrdinalIgnoreCase) ?? false
                    ? Theme.Dark
                    : Theme.Light;
            }
        }
    }
}
