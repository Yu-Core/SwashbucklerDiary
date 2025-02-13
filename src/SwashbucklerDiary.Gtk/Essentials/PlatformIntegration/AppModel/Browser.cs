using System.Diagnostics;

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

        static Task<bool> GTKTryOpenAsync(Uri uri)
        {
            try
            {
                using var process = new Process();

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = "xdg-open";
                process.StartInfo.Arguments = uri.ToString();
                bool isSuccess = process.Start();
                return Task.FromResult(isSuccess);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}
