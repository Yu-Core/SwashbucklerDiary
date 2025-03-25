using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Shared
{
    public static class StringExtensions
    {
        public static string MD5Encrytp32(this string? password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            byte[] newBuffer = MD5.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string TrimStart(this string str, string trimString)
        {
            ArgumentNullException.ThrowIfNull(str);

            ArgumentNullException.ThrowIfNull(trimString);

            if (str.StartsWith(trimString))
                return str.Substring(trimString.Length);

            return str;
        }
    }
}
