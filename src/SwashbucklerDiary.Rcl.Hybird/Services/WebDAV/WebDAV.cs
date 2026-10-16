using System.Net;
using WebDav;

namespace SwashbucklerDiary.Rcl.Services
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

        public async Task<List<WebDAVFileInfo>> GetZipFileListAsync(string folderName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            var result = await webDavClient.Propfind($"{webDavFolderName}/");
            if (result.IsSuccessful)
            {
                return result.Resources
                    .Where(it => !it.IsCollection)
                    .Where(it => it.Uri.EndsWith(".zip"))
                    .OrderByDescending(it => it.LastModifiedDate)
                    .Select(it => new WebDAVFileInfo()
                    {
                        Name  = Uri.UnescapeDataString(Path.GetFileName(it.Uri)),
                        Length = it.ContentLength,
                        LastModified = it.LastModifiedDate
                    })
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

            Uri uri = NormalizeBaseAddress(baseAddress);

            var webDavClient = GetWebDavClient(uri, userName, password);
            var result = await webDavClient.Propfind($"{webDavFolderName}/");
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

        /// <summary>
        /// HttpClient.BaseAddress 不以 / 结尾时，相对路径会替换掉最后一段路径
        /// （例如 http://host/dav + SwashbucklerDiary/ => http://host/SwashbucklerDiary/），
        /// 导致请求发到 WebDAV 目录之外，服务器返回 404/405。这里统一补上结尾的 /。
        /// </summary>
        private static Uri NormalizeBaseAddress(string? baseAddress)
        {
            if (!Uri.TryCreate(baseAddress?.Trim(), UriKind.Absolute, out Uri? uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException(null, nameof(baseAddress));
            }

            if (uri.AbsolutePath.EndsWith('/'))
            {
                return uri;
            }

            var builder = new UriBuilder(uri);
            builder.Path += "/";
            return builder.Uri;
        }

        private static WebDavClient GetWebDavClient(Uri uri, string userName, string password)
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
