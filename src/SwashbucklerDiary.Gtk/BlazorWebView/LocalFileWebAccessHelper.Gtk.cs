using GtkSharp.BlazorWebKit;
using Soup;
using SwashbucklerDiary.Shared;
using WebKit;

namespace SwashbucklerDiary.Gtk.BlazorWebView
{
    public static partial class LocalFileWebAccessHelper
    {
        public static bool InterceptCustomPathRequest(URISchemeRequest request)
        {
            string uri = request.GetUri();
            if (!InterceptLocalFileRequest(uri, out string filePath))
            {
                return false;
            }

            using var contentStream = File.OpenRead(filePath);
            string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(filePath);
            var length = contentStream.Length;
            int statusCode = 200;
            string statusMessage = "OK";
            long rangeStart = 0;
            long rangeEnd = length - 1;
            var messageHeaders = MessageHeaders.New(MessageHeadersType.Response);

            //适用于音频视频文件资源的响应
            string? rangeString = request.GetHttpHeaders().GetOne("Range");
            if (!string.IsNullOrEmpty(rangeString))
            {
                //206,可断点续传
                statusCode = 206;
                statusMessage = "Partial Content";

                ParseRange(rangeString, ref rangeStart, ref rangeEnd);
                messageHeaders.Append("Accept-Ranges", "bytes");
                messageHeaders.Append("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{length}");
            }

            var bytes = ReadStreamRange(contentStream, rangeStart, rangeEnd);
            var inputStream = InputStreamNewFromBytes(bytes);
            var response = URISchemeResponse.New(inputStream, bytes.LongLength);

            response.SetStatus((uint)statusCode, statusMessage);
            messageHeaders.SetContentLength(bytes.LongLength);
            // Disable local caching. This will prevent user scripts from executing correctly.
            messageHeaders.Append("Cache-Control", "no-cache, max-age=0, must-revalidate, no-store");
            messageHeaders.Append("Content-Type", contentType);
            response.SetHttpHeaders(messageHeaders);

            request.FinishWithResponse(response);

            inputStream?.Dispose();
            return true;
        }

        private static bool InterceptLocalFileRequest(string uri, out string filePath)
        {
            if (!uri.StartsWith(GtkWebViewManager.AppOriginUri.ToString()))
            {
                filePath = string.Empty;
                return false;
            }

            var urlRelativePath = new Uri(uri).AbsolutePath.TrimStart('/');
            filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);
            if (!File.Exists(filePath))
            {
                return false;
            }

            return true;
        }

        private static int ParseRange(string rangeString, ref long rangeStart, ref long rangeEnd)
        {
            var ranges = rangeString.Split('=');
            if (ranges.Length < 2 || string.IsNullOrEmpty(ranges[1]))
            {
                return 0;
            }

            string[] rangeDatas = ranges[1].Split("-");
            rangeStart = Convert.ToInt64(rangeDatas[0]);
            if (rangeDatas.Length > 1 && !string.IsNullOrEmpty(rangeDatas[1]))
            {
                rangeEnd = Convert.ToInt64(rangeDatas[1]);
                return 2;
            }
            else
            {
                return 1;
            }
        }

        static byte[] ReadStreamRange(FileStream contentStream, long start, long end)
        {
            // 检查结束位置是否大于开始位置
            if (end < start)
            {
                throw new ArgumentException("结束位置必须大于开始位置");
            }

            // 计算需要读取的字节数
            long numberOfBytesToRead = end - start + 1;
            byte[] byteArray = new byte[numberOfBytesToRead];
            contentStream.Seek(start, SeekOrigin.Begin);
            int bytesRead = contentStream.Read(byteArray, 0, (int)(numberOfBytesToRead));
            // 如果读取的字节数小于期望的字节数，说明到达了文件的末尾或发生了其他错误
            if (bytesRead < numberOfBytesToRead)
            {
                // 创建一个新的缓冲区，只包含实际读取的字节
                byte[] actualBuffer = new byte[bytesRead];
                Array.Copy(byteArray, actualBuffer, bytesRead);
                return actualBuffer;
            }
            return byteArray;
        }

        static Gio.MemoryInputStream InputStreamNewFromBytes(byte[] buffer)
        {
            var bytes = GLib.Bytes.New(buffer);
            return Gio.MemoryInputStream.NewFromBytes(bytes);
        }
    }
}
