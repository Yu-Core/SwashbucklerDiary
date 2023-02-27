using System.Security.Cryptography;
using System.Text;

namespace SwashbucklerDiary.Extend
{
    public static class StringExtend
    {
        public static string MD5Encrytp32(this string password)
        {
            byte[] newBuffer = MD5.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
