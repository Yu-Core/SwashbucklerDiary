using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class LocalImageService : ILocalImageService
    {
        private readonly IJSRuntime JS;
        private IJSObjectReference module = default!;
        private const string flag = "local:///";
        private Dictionary<string, string> uriToUrl = new ();

        public LocalImageService(IJSRuntime jS) 
        {
            JS = jS;
        }

        public async Task<string> AddFlag(string uri)
        {
            await InitModule();
            return flag + uri;
        }

        public async Task<string> ToUrl(string uri)
        {
            await InitModule();
            if(string.IsNullOrEmpty(uri))
            {
                return uri;
            }

            if(!uri.StartsWith(flag))
            {
                return uri;
            }

            uri = uri.Substring(flag.Length);

            if (uriToUrl.TryGetValue(uri, out string? value))
            {
                return value;
            }

            using var imageStream = File.OpenRead(uri);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            var url = await module.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
            uriToUrl.Add(uri,url);
            return url;
        }

        public async Task RevokeUrl(string url)
        {
            await InitModule();
            if(uriToUrl.ContainsValue(url))
            {
                uriToUrl.Remove(uriToUrl.FirstOrDefault(it=>it.Value == url).Value);
                await module.InvokeVoidAsync("revokeUrl", new object[1] { url });
            }
        }

        private async Task InitModule()
        {
            module ??= await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getLocalImage.js");
        }
    }
}
