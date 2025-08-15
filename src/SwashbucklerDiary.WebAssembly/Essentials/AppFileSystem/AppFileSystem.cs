using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppFileSystem : Rcl.Essentials.AppFileSystem
    {
        private readonly IJSRuntime _jSRuntime;

        public AppFileSystem(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }
        public override string AppDataDirectory => FileSystem.AppDataDirectory;

        public override string CacheDirectory => FileSystem.CacheDirectory;

        public override async Task SyncFS()
        {
            //立即从memfs(内存)同步到idbfs(indexedDB)，持久化，使拦截请求(service worker)能够找到文件
            await _jSRuntime.InvokeVoidAsync("WasmFileSystem.syncfs").ConfigureAwait(false);
        }
    }
}
