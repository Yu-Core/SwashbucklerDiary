namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> OpenBrowser(string? uriString)
        {
            return OpenLauncher(uriString);
        }

        public static async Task<bool> OpenLauncher(string? uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                return await GTKTryOpenAsync(uri);
            }

            return false;
        }

        // from https://github.com/jsuarezruiz/maui-linux/blob/main/src/Essentials/src/Launcher/Launcher.Gtk.cs
        static async Task<bool> GTKTryOpenAsync(Uri uri)
        {
            string stdout, stderr;
            int exitCode;
            var task = Task.Run(
                () => GLib.Process.SpawnCommandLineSync("xdg-open " + uri.ToString(), out stdout, out stderr, out exitCode));
            var result = await task;
            return result;
        }
    }
}
