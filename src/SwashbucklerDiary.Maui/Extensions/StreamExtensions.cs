using System.Security.Cryptography;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class StreamExtensions
    {
        public static string CreateMD5(this Stream stream, int bufferSize = 1024 * 1024)
        {
            using MD5 md5 = MD5.Create();
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            }

            md5.TransformFinalBlock(buffer, 0, 0);

            stream.Seek(0, SeekOrigin.Begin);

            byte[] hash = md5.Hash ?? [];
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
