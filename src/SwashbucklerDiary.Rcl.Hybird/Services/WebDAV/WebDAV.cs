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
            => await GetFileListAsync(folderName, ".zip");

        public async Task<List<WebDAVFileInfo>> GetFileListAsync(string folderName, string? extension = null)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            string path = NormalizeFolderPath(folderName);
            var result = await webDavClient.Propfind(path);
            if (result.IsSuccessful)
            {
                return result.Resources
                    .Where(it => !string.IsNullOrEmpty(it.Uri))
                    .Select(it => new { Resource = it, Uri = it.Uri! })
                    .Where(it => !IsSameResource(it.Uri, path, it.Resource.IsCollection))
                    .Where(it => extension is null || (!it.Resource.IsCollection && it.Uri.EndsWith(extension, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(it => it.Resource.LastModifiedDate)
                    .Select(it => new WebDAVFileInfo()
                    {
                        Name = GetResourceName(it.Uri),
                        Length = it.Resource.ContentLength,
                        LastModified = it.Resource.LastModifiedDate,
                        IsCollection = it.Resource.IsCollection
                    })
                    .ToList();
            }
            else
            {
                throw new WebDAVException(result.ToString());
            }
        }

        public async Task EnsureFolderAsync(string folderName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            string current = string.Empty;
            foreach (string part in folderName.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                current = string.IsNullOrEmpty(current) ? part : $"{current}/{part}";
                var result = await webDavClient.Propfind($"{current}/");
                if (result.IsSuccessful)
                {
                    continue;
                }

                if (result.StatusCode != (int)HttpStatusCode.NotFound)
                {
                    throw new WebDAVException(result.ToString());
                }

                var mkcolResult = await webDavClient.Mkcol(current);
                if (!mkcolResult.IsSuccessful && mkcolResult.StatusCode != (int)HttpStatusCode.MethodNotAllowed)
                {
                    throw new WebDAVException(mkcolResult.ToString());
                }
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

        public async Task<bool> FileExistsAsync(string fileName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            var result = await webDavClient.Propfind(fileName);
            if (result.IsSuccessful)
            {
                return true;
            }

            if (result.StatusCode == (int)HttpStatusCode.NotFound)
            {
                return false;
            }

            throw new WebDAVException(result.ToString());
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

        private static string NormalizeFolderPath(string folderName)
            => folderName.EndsWith('/') ? folderName : $"{folderName}/";

        private static bool IsSameResource(string uri, string folderName, bool isCollection)
            => isCollection && GetResourceName(uri) == GetResourceName(folderName);

        private static string GetResourceName(string uri)
            => Uri.UnescapeDataString(uri.TrimEnd('/').Split('/').LastOrDefault() ?? string.Empty);

    }
}
