using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class LocalImageService : ILocalImageService
    {
        private readonly IJSRuntime JS = default!;
        private IJSObjectReference module = default!;
        private const string flag = "local:";
        private List<string> urls = new ();

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
            using var imageStream = File.OpenRead(uri);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            var url = await module.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
            urls.Add(url);
            return url;
        }

        public async Task RevokeUrl(string url)
        {
            await InitModule();
            if(urls.Any(it=>it == url))
            {
                urls.Remove(url);
                await module.InvokeVoidAsync("revokeUrl", new object[1] { url });
            }
        }

        private async Task InitModule()
        {
            module ??= await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getNativeImage.js");
        }
    }
}
