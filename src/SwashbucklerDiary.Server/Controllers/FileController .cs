using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Storage;
using Microsoft.Net.Http.Headers;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Server.Attributes;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Server.Controllers
{
    [ApiAuth]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _appDataDir;
        private readonly string _cacheDir;

        public FileController(IMediaResourceManager mediaResourceManager)
        {
            _appDataDir = mediaResourceManager.AssetsDirectoryPath;
            _cacheDir = FileSystem.CacheDirectory;
        }

        [HttpGet("appdata/{**path}")]
        public IActionResult GetAppData(string path)
            => ServeFile(_appDataDir, path);

        [HttpGet("cache/{**path}")]
        public IActionResult GetCache(string path)
            => ServeFile(_cacheDir, path);

        private IActionResult ServeFile(string baseDir, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return BadRequest();

            // 防目录穿越
            var fullPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));
            if (!fullPath.StartsWith(Path.GetFullPath(baseDir)))
                return Forbid();

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var fileInfo = new FileInfo(fullPath);
            var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(fullPath);

            // Range 处理
            if (Request.Headers.ContainsKey(HeaderNames.Range))
            {
                return PhysicalFile(
                    fullPath,
                    contentType,
                    enableRangeProcessing: true
                );
            }

            return PhysicalFile(
                fullPath,
                contentType
            );
        }
    }

}
