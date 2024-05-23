using SwashbucklerDiary.Rcl.Essentials;
using System.Net;
using WebDav;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class WebDAV : IWebDAV
    {
        private WebDavClient? webDavClient;

        private const string webDavFolderName = "SwashbucklerDiary";

        public bool Initialized { get; set; }

        public async Task<Stream> DownloadAsync(string destFileName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            var response = await webDavClient.GetRawFile(destFileName);
            if (response.IsSuccessful)
            {
                return response.Stream;
            }
            else
            {
                throw new WebDAVException(response.ToString());
            }
        }

        public async Task<List<string>> GetZipFileListAsync(string folderName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            var result = await webDavClient.Propfind(folderName);
            if (result.IsSuccessful)
            {
                return result.Resources
                    .Where(it => !it.IsCollection)
                    .Where(it => Path.GetExtension(it.DisplayName) == ".zip")
                    .OrderByDescending(it => it.LastModifiedDate)
                    .Select(it => it.DisplayName)
                    .ToList();
            }
            else
            {
                throw new WebDAVException(result.ToString());
            }
        }

        public async Task Set(string? baseAddress, string? userName, string? password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseAddress, nameof(baseAddress));
            ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(userName));
            ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

            bool uriResult = Uri.TryCreate(baseAddress, UriKind.Absolute, out Uri? uri);
            if (!uriResult || uri is null)
            {
                throw new ArgumentException(null, nameof(baseAddress));
            }

            var webDavClient = GetWebDavClient(uri, userName, password);
            var result = await webDavClient.Propfind(webDavFolderName);
            if (!result.IsSuccessful)
            {
                if (result.StatusCode != (int)HttpStatusCode.NotFound)
                {
                    throw new WebDAVException(result.ToString());
                }

                var result2 = await webDavClient.Mkcol(webDavFolderName);
                if (!result2.IsSuccessful)
                {
                    throw new WebDAVException(result2.ToString());
                }
            }

            this.webDavClient = webDavClient;
            Initialized = true;
        }

        public async Task UploadAsync(string destFileName, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            var result = await webDavClient.PutFile(destFileName, stream);
            if (!result.IsSuccessful)
            {
                throw new WebDAVException(result.ToString());
            }
        }


        private WebDavClient GetWebDavClient(Uri uri, string userName, string password)
        {
            var httpHandler = new SocketsHttpHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = new NetworkCredential(userName, password)
            };
            var client = new HttpClient(httpHandler, true) { BaseAddress = uri };
            return new WebDavClient(client);
        }

    }
}
