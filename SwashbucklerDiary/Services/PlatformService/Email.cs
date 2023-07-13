using SwashbucklerDiary.IServices;
#if WINDOWS
using System.Text;
#endif

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService : IPlatformService
    {
        public bool IsMailSupported()
        {
            return Email.Default.IsComposeSupported;
        }

        public Task SendEmail(List<string>? recipients) => SendEmail(null, null, recipients);

        public Task SendEmail(string? subject, string? body, List<string>? recipients)
        {
#if WINDOWS
            return WindowsSendEmail(subject,body,recipients);
#else
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients
            };
            return Email.Default.ComposeAsync(message);
#endif
        }

#if WINDOWS
        //There are some problems in Windows. https://github.com/dotnet/maui/issues/11218
        private static Task WindowsSendEmail(string? subject, string? body, List<string>? recipients)
        {
            StringBuilder emailAddress = new();
            if(recipients != null)
            {
                foreach (string mail in recipients)
                {
                    emailAddress.Append(mail);
                    emailAddress.Append(';');
                }
                emailAddress.Length--;
            }

            string mailtoLink = $"mailto:{emailAddress}?subject={subject}&body={body}";
            return Launcher.Default.OpenAsync(mailtoLink);
        }
#endif
    }
}
