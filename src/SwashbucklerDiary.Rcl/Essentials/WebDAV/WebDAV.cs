using System.Net;
using WebDav;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public class WebDAV : IWebDAV
    {
        private WebDavClient? webDavClient;

        private const string webDavFolderName = "SwashbucklerDiary";

        public bool Initialized { get; set; }

        public async Task<Stream> DownloadAsync(string destFileName)
        {
            ArgumentNullException.ThrowIfNull(webDavClient, nameof(webDavClient));

            using var response = await webDavClient.GetRawFile(destFileName);
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

            var result = await webDavClient.Mkcol(webDavFolderName);
            if (result.IsSuccessful)
            {
                this.webDavClient = webDavClient;
                Initialized = true;
            }
            else
            {
                throw new WebDAVException(result.ToString());
            }
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

        protected virtual WebDavClient GetWebDavClient(Uri uri, string userName, string password)
        {
            var clientParams = new WebDavClientParams
            {
                BaseAddress = uri,
                Credentials = new NetworkCredential(userName, password)
            };
            return new WebDavClient(clientParams);
        }
    }
}
