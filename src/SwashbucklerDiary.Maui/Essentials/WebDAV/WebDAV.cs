using System.Net;
using WebDav;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class WebDAV : Rcl.Essentials.WebDAV
    {
#if ANDROID
        protected override WebDavClient GetWebDavClient(Uri uri, string userName, string password)
        {
            var httpHandler = new SocketsHttpHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = new NetworkCredential(userName, password)
            };
            var client = new HttpClient(httpHandler, true) { BaseAddress = uri };
            return new WebDavClient(client);
        }
#endif
    }
}
