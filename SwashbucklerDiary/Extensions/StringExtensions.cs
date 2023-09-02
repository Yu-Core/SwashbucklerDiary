using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Extend
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

        public static bool IsRelativePath(this string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Relative);
        }
    }
}
