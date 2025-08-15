using System.Security.Cryptography;

namespace SwashbucklerDiary.Shared
{
    public static class StreamExtensions
    {
        public static async Task<string> CreateMD5(this Stream stream, int bufferSize = 1024 * 1024)
        {
            using MD5 md5 = MD5.Create();
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
            {
                md5.TransformBlock(buffer, 0, bytesRead, null, 0);
            }

            md5.TransformFinalBlock([], 0, 0);

            stream.Seek(0, SeekOrigin.Begin);

            byte[] hash = md5.Hash ?? [];
            return Convert.ToHexStringLower(hash);
        }
    }
}
