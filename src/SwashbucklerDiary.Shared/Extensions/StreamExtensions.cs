using System.Buffers;
using System.Security.Cryptography;

namespace SwashbucklerDiary.Shared
{
    public static class StreamExtensions
    {
        public static async Task<string> CreateSHA256Async(
            this Stream stream,
            int bufferSize = 1024 * 1024,
            CancellationToken cancellationToken = default)
        {
            using var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize); // 使用缓冲池减少GC压力

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    incrementalHash.AppendData(buffer, 0, bytesRead);
                }

                byte[] hash = incrementalHash.GetHashAndReset();
                return Convert.ToHexStringLower(hash);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer); // 确保归还缓冲区
            }
        }
    }
}
