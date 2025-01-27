using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class GtkSystemThemeManager
    {
        public Theme SystemTheme { get; set; }

        public event Action<Theme>? SystemThemeChanged;

        public void Initialized()
        {
            SystemTheme = GetCurrentSystemTheme();

            Adw.StyleManager.GetDefault().OnNotify += OnSystemThemeChanged;
            //global::Gtk.Settings.GetDefault().OnNotify += OnSystemThemeChanged;
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

        private static Theme GetCurrentSystemTheme()
        {
            return Adw.StyleManager.GetDefault().Dark ? Theme.Dark : Theme.Light;
        }
    }
}
