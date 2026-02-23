using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Security.Cryptography;
using System.Text;

namespace SwashbucklerDiary.Server.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private string? _secretKey;
        private string? _version;

        readonly IPreferences preferences;

        static readonly string sharedName = AppInfo.Current.PackageName + "apiauth";

        public ApiAuthService()
        {
            this.preferences = Preferences.Default;
            Init();
        }

        void Init()
        {
            _secretKey = preferences.Get<string?>("secretKey", null, sharedName);
            if (_secretKey is null)
            {
                _secretKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                preferences.Set("secretKey", _secretKey, sharedName);
            }

            _version = preferences.Get<string?>("version", null, sharedName);
            if (_version is null)
            {
                _version = "v1";
                preferences.Set("version", _version, sharedName);
            }
        }

        public void UpdateVersion()
        {
            _version = $"v{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            preferences.Set("version", _version, sharedName);
        }

        public string GenerateApiKey()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var nonce = Guid.NewGuid().ToString("N");

            var payload = $"{_version}|{timestamp}|{nonce}";
            var signature = ComputeHmac(payload);

            var finalValue = $"{payload}|{signature}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(finalValue));
        }

        public bool ValidateApiKey(string apiKey)
        {
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(apiKey));
                var parts = decoded.Split('|');

                if (parts.Length != 4)
                    return false;

                var version = parts[0];
                var timestamp = parts[1];
                var nonce = parts[2];
                var signature = parts[3];

                // 1️⃣ 版本校验（版本变了全部失效）
                if (version != _version)
                    return false;

                // 2️⃣ 过期校验（例如 30 分钟）
                //var time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp));
                //if (DateTimeOffset.UtcNow - time > TimeSpan.FromMinutes(30))
                //    return false;

                // 3️⃣ 签名校验
                var payload = $"{version}|{timestamp}|{nonce}";
                var expectedSignature = ComputeHmac(payload);

                return signature == expectedSignature;
            }
            catch
            {
                return false;
            }
        }

        private string ComputeHmac(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}
