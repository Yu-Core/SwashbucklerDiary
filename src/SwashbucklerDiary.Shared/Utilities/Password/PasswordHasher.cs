using System.Security.Cryptography;

namespace SwashbucklerDiary.Shared
{
    public class PasswordHasher
    {
        // 生成随机盐
        public static byte[] GenerateRandomSalt(int saltLength = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[saltLength];
                rng.GetBytes(salt);
                return salt;
            }
        }

        // 使用 PBKDF2 哈希密码
        public static byte[] HashPassword(string password, byte[] salt, int iterations = 10000, int hashLength = 32)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: hashLength
            );
        }

        // 将字节数组转换为Base64字符串
        public static string BytesToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        // 将Base64字符串转换为字节数组
        public static byte[] Base64ToBytes(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        // 完整密码哈希流程
        public static string HashPasswordWithSalt(string password, out string saltBase64)
        {
            // 生成随机盐
            var salt = GenerateRandomSalt();
            saltBase64 = BytesToBase64(salt);

            // 哈希密码
            var hash = HashPassword(password, salt);

            // 返回哈希结果的Base64字符串
            return BytesToBase64(hash);
        }

        // 验证密码
        public static bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64)
        {
            var salt = Base64ToBytes(storedSaltBase64);
            var storedHash = Base64ToBytes(storedHashBase64);

            // 使用相同的盐和参数哈希输入的密码
            var computedHash = HashPassword(password, salt);

            // 比较计算出的哈希与存储的哈希
            return SlowEquals(computedHash, storedHash);
        }

        // 安全比较两个字节数组（防止时序攻击）
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
