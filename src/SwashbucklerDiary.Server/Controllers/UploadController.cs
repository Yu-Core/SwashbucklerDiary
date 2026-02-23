using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Storage;
using SwashbucklerDiary.Server.Attributes;

namespace SwashbucklerDiary.Server.Controllers
{
    [ApiAuth]
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        [RequestSizeLimit(1024 * 1024 * 1024)] // 1GB，按需改
        [HttpPost]
        public async Task<IActionResult> UploadMulti(List<IFormFile> files)
        {
            if (files == null)
                return BadRequest("Parameter files are missing");

            if (files.Count == 0)
                return Ok(Array.Empty<string>());

            // 缓存根目录
            string cacheRoot = FileSystem.CacheDirectory;

            var paths = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                // 重新生成文件名
                string fileName = file.FileName;
                string folderName = Guid.NewGuid().ToString("N");
                string folderPath = Path.Combine(cacheRoot, folderName);
                Directory.CreateDirectory(folderPath);
                string filePath = Path.Combine(folderPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                paths.Add(filePath);
            }

            return Ok(paths.ToArray());
        }
    }

}
