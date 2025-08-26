using EmbedIO;
using EmbedIO.Files;
using Swan.Logging;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public class LocalFileModule : FileModule
    {
        public LocalFileModule(string baseRoute, IFileProvider provider) : base(baseRoute, provider)
        {
        }

        protected override async Task OnRequestAsync(IHttpContext context)
        {
            MappedResourceInfo? info;

            var path = context.RequestedPath;
            info = MapUrlPath(path, context);
            if (info == null)
            {
                // If mapping failed, send a "404 Not Found" response, or whatever OnMappingFailed chooses to do.
                // For example, it may return a default resource (think a folder of images and an imageNotFound.jpg),
                // or redirect the request.
                await OnMappingFailed(context, null).ConfigureAwait(false);
            }
            else if (!IsHttpMethodAllowed(context.Request, out var sendResponseBody))
            {
                // If there is a mapped resource, check that the HTTP method is either GET or HEAD.
                // Otherwise, send a "405 Method Not Allowed" response, or whatever OnMethodNotAllowed chooses to do.
                await OnMethodNotAllowed(context, info).ConfigureAwait(false);
            }
            else if (info.IsDirectory && DirectoryLister == null)
            {
                // If a directory listing was requested, but there is no DirectoryLister,
                // send a "403 Unauthorized" response, or whatever OnDirectoryNotListable chooses to do.
                // For example, one could prefer to send "404 Not Found" instead.
                await OnDirectoryNotListable(context, info).ConfigureAwait(false);
            }
            else
            {
                await HandleResource(context, info).ConfigureAwait(false);
            }
        }

        private MappedResourceInfo? MapUrlPath(string urlPath, IMimeTypeProvider mimeTypeProvider)
        {
            var result = Provider.MapUrlPath(urlPath, mimeTypeProvider);

            // If urlPath maps to a file, no further searching is needed.
            if (result?.IsFile ?? false)
                return result;

            // Look for a default document.
            // Don't append an additional slash if the URL path is "/".
            // The default document, if found, must be a file, not a directory.
            if (DefaultDocument != null)
            {
                var defaultDocumentPath = urlPath + (urlPath.Length > 1 ? "/" : string.Empty) + DefaultDocument;
                var defaultDocumentResult = Provider.MapUrlPath(defaultDocumentPath, mimeTypeProvider);
                if (defaultDocumentResult?.IsFile ?? false)
                    return defaultDocumentResult;
            }

            // Try to apply default extension (but not if the URL path is "/",
            // i.e. the only normalized, non-base URL path that ends in a slash).
            // When the default extension is applied, the result must be a file.
            if (DefaultExtension != null && urlPath.Length > 1)
            {
                var defaultExtensionResult = Provider.MapUrlPath(urlPath + DefaultExtension, mimeTypeProvider);
                if (defaultExtensionResult?.IsFile ?? false)
                    return defaultExtensionResult;
            }

            return result;
        }

        private static bool IsHttpMethodAllowed(IHttpRequest request, out bool sendResponseBody)
        {
            switch (request.HttpVerb)
            {
                case HttpVerbs.Head:
                    sendResponseBody = false;
                    return true;
                case HttpVerbs.Get:
                    sendResponseBody = true;
                    return true;
                default:
                    sendResponseBody = default;
                    return false;
            }
        }

        public async Task HandleResource(IHttpContext context, MappedResourceInfo info)
        {
            var path = info.Path;
            var length = info.Length;
            var response = context.Response;
            var request = context.Request;

            // 设置通用响应头
            response.ContentType = info.ContentType ?? DirectoryLister?.ContentType ?? MimeType.Default;
            response.Headers.Set("Accept-Ranges", "bytes");

            // 检查是否是范围请求
            var rangeHeader = request.Headers["Range"];
            if (string.IsNullOrEmpty(rangeHeader))
            {
                // 不是范围请求，返回整个文件
                response.StatusCode = 200;
                response.ContentLength64 = length;

                using var fileStream = File.OpenRead(path);
                await fileStream.CopyToAsync(response.OutputStream);
            }
            else
            {
                // 处理范围请求
                var range = ParseRange(rangeHeader, length);
                if (range == null)
                {
                    response.StatusCode = 416; // 请求范围不符合要求
                    response.Headers.Set("Content-Range", $"bytes */{length}");
                    return;
                }

                response.StatusCode = 206; // 部分内容
                response.Headers.Set("Content-Range", $"bytes {range.Start}-{range.End}/{length}");
                response.ContentLength64 = range.Length;

                using var fileStream = File.OpenRead(path);
                fileStream.Seek(range.Start, SeekOrigin.Begin);

                var buffer = new byte[81920];
                var remaining = range.Length;

                while (remaining > 0)
                {
                    var bytesRead = fileStream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (bytesRead == 0) break;

                    await response.OutputStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    remaining -= bytesRead;
                }
            }
        }

        private static Range? ParseRange(string rangeHeader, long fileSize)
        {
            try
            {
                // 示例 Range 头: "bytes=0-499", "bytes=500-999", "bytes=-500", "bytes=9500-"
                var range = rangeHeader.Replace("bytes=", "");
                var parts = range.Split('-');

                long start = 0;
                long end = fileSize - 1;

                if (parts.Length == 2)
                {
                    if (!string.IsNullOrEmpty(parts[0]))
                        start = long.Parse(parts[0]);

                    if (!string.IsNullOrEmpty(parts[1]))
                        end = long.Parse(parts[1]);
                    else
                        end = fileSize - 1;

                    // 处理格式如 "bytes=-500" (最后500字节)
                    if (range.StartsWith('-'))
                    {
                        start = fileSize - long.Parse(parts[1]);
                        end = fileSize - 1;
                    }

                    // 确保范围有效
                    if (start < 0) start = 0;
                    if (end >= fileSize) end = fileSize - 1;

                    if (start > end) return null; // 无效范围

                    return new Range { Start = start, End = end, Length = end - start + 1 };
                }
            }
            catch (Exception ex)
            {
                $"Error parsing range header: {ex.Message}".Error();
            }

            return null;
        }

        private class Range
        {
            public long Start { get; set; }
            public long End { get; set; }
            public long Length { get; set; }
        }
    }

}
