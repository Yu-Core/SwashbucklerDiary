using SwashbucklerDiary.Rcl.Essentials;
using System.Text;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IPlatformIntegrationExtensions
    {
        public static string CreateMailToUri(this IPlatformIntegration _, string? subject, string? body, List<string>? recipients)
        {
            StringBuilder emailAddress = new();
            if (recipients != null)
            {
                foreach (string mail in recipients)
                {
                    emailAddress.Append(mail);
                    emailAddress.Append(';');
                }
                emailAddress.Length--;
            }

            return $"mailto:{emailAddress}?subject={subject}&body={body}";
        }
    }
}
