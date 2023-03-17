using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using System.Collections.Concurrent;

namespace SwashbucklerDiary.Services
{
    public class LocalImageService : ILocalImageService
    {
        private readonly IJSRuntime JS;
        private IJSObjectReference module = default!;
        private const string flag = "local:///";
        private static readonly ConcurrentDictionary<string, string> urls = new ();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

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

            SemaphoreSlim semaphore = semaphores.GetOrAdd(uri, _ => new SemaphoreSlim(1));
            await semaphore.WaitAsync();

            try
            {
                if (urls.TryGetValue(uri, out string? url))
                {
                    return url;
                }
                else
                {
                    string newUrl = await GenerateImageUrl(uri);
                    urls.TryAdd(uri, newUrl);

                    return newUrl;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<string> GenerateImageUrl(string uri)
        {
            using var imageStream = File.OpenRead(uri);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            var url = await module.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
            return url;
        }

        public async Task RevokeUrl(string uri)
        {
            await InitModule();
            if (!uri.StartsWith(flag))
            {
                return;
            }

            uri = uri.Substring(flag.Length);

            if (urls.ContainsKey(uri))
            {
                urls.TryRemove(uri,out string? url);
                await module.InvokeVoidAsync("revokeUrl", new object[1] { url! });
            }
        }

        private async Task InitModule()
        {
            module ??= await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getLocalImage.js");
        }
    }
}
