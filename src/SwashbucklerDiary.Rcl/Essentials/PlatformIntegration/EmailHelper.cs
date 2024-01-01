using System.Text;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public static class EmailHelper
    {
        public static string CreateMailToUri(string? subject, string? body, List<string>? recipients)
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
