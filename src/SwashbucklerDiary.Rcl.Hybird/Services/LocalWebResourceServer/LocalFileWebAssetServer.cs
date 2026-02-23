using EmbedIO;
using EmbedIO.Cors;
using SwashbucklerDiary.Rcl.Hybird.Extensions;
using System.Net;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public class LocalFileWebAssetServer : IDisposable
    {
        const string basePath = "http://localhost";
        private WebServer? server;

        public LocalFileWebAssetServer(Dictionary<string, string> routeFileSystemPathMap)
        {
            StartWebServer(routeFileSystemPathMap);
        }

        public string UrlPrefix { get; } = $"{basePath}:{IPAddress.Loopback.GetAvailablePort()}";

        private void StartWebServer(Dictionary<string, string> routeFileSystemPathMap)
        {
            if (server is not null)
            {
                return;
            }

            server = new WebServer(i => i
            .WithUrlPrefix(UrlPrefix))
            .WithCors(CorsModule.All)
            .WithStaticFolder(routeFileSystemPathMap);

            server.RunAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                server?.Dispose();
                server = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
