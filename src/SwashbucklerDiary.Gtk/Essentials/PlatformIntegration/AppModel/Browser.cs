using System.Diagnostics;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> OpenBrowser(string? uriString)
        {
            return OpenLauncher(uriString);
        }

        public static Task<bool> OpenLauncher(string? uriString)
        {
            bool isSuccess = false;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                isSuccess = GTKTryOpen(uri);

            }

            return Task.FromResult(isSuccess);
        }

        static bool GTKTryOpen(Uri uri)
        {
            try
            {
                using var process = new Process();

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = "xdg-open";
                process.StartInfo.Arguments = uri.ToString();
                bool isSuccess = process.Start();
                return isSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
