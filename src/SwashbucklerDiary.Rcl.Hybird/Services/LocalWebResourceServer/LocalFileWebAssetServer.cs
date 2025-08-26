using EmbedIO;
using EmbedIO.Cors;
using System.Net;
using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public class LocalFileWebAssetServer : IDisposable
    {
        const string basePath = "http://localhost";
        private WebServer? server;
        private CancellationTokenSource? cts;

        public LocalFileWebAssetServer(Dictionary<string, string> routeFileSystemPathMap)
        {
            StartWebServer(routeFileSystemPathMap);
        }

        public string UrlPrefix { get; } = $"{basePath}:{GetAvailablePort(IPAddress.Loopback)}";

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
            cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                server.RunAsync(cts.Token);
            }, cts.Token);
        }

        static int GetAvailablePort(IPAddress ip)
        {
            using TcpListener l = new TcpListener(ip, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                cts?.Cancel();
                server?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
