using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Extensions
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

        public static string FileMD5(this string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static int WordCount(this string s)
        {
            string englishExpression = @"[\S]+";
            MatchCollection collection = Regex.Matches(s, englishExpression);
            return collection.Count;
        }

        public static int CharacterCount(this string s)
        {
            string asianExpression = @"[\u3001-\uFFFF]";
            MatchCollection asiancollection = Regex.Matches(s, asianExpression);
            var count = asiancollection.Count; //Asian Character Count
            s = Regex.Replace(s, asianExpression, " ");

            string englishExpression = @"[\S]+";
            MatchCollection collection = Regex.Matches(s, englishExpression);
            foreach (Match word in collection.Cast<Match>())
            {
                count += word.Value.Length;
            }
            return count;
        }

        public static bool EqualsAbsolutePath(this string? uri1,string? uri2)
        {
            if (uri1 == null || uri2 == null)
            {
                return false;
            }

            return new Uri(uri1).AbsolutePath == new Uri(uri2).AbsolutePath;
        }

        public static string TrimStart(this string str, string trimString)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (trimString == null)
                throw new ArgumentNullException(nameof(trimString));

            if (str.StartsWith(trimString))
                return str.Substring(trimString.Length);

            return str;
        }
    }
}
