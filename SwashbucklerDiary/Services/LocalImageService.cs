using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using System.Collections.Concurrent;

namespace SwashbucklerDiary.Services
{
    public class LocalImageService : ILocalImageService
    {
        private readonly IJSRuntime JS;
        private IJSObjectReference module = default!;
        //存储已生成的图片，将图片本机路径与图片blob的url联系起来
        private static readonly ConcurrentDictionary<string, string> urls = new ();
        //控制访问单个图片资源的线程数量，若图片blob在生成中，将等待
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> semaphores = new();

        public LocalImageService(IJSRuntime jS) 
        {
            JS = jS;
        }

        //为本地路径的图片创建blob,并返回blob的url,若不是本地路径会直接返回
        public async Task<string> ToUrl(string path)
        {
            await InitModule();
            if(!File.Exists(path))
            {
                return path;
            }

            SemaphoreSlim semaphore = semaphores.GetOrAdd(path, _ => new SemaphoreSlim(1));
            await semaphore.WaitAsync();

            try
            {
                if (urls.TryGetValue(path, out string? url))
                {
                    return url;
                }
                else
                {
                    string newUrl = await GenerateImageUrl(path);
                    urls.TryAdd(path, newUrl);

                    return newUrl;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
        //调用js，生成图片的blob
        private async Task<string> GenerateImageUrl(string path)
        {
            using var imageStream = File.OpenRead(path);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            var url = await module.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
            return url;
        }
        //调用js，释放图片的blob
        public async Task RevokeUrl(string path)
        {
            await InitModule();

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (urls.ContainsKey(path))
            {
                urls.TryRemove(path,out string? url);
                await module.InvokeVoidAsync("revokeUrl", new object[1] { url! });
            }
        }

        //初始化JS模块
        private async Task InitModule()
        {
            module ??= await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getLocalImage.js");
        }
    }
}
