#if WINDOWS
using SwashbucklerDiary.Rcl.Extensions;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public bool IsMailSupported()
        {
            return Email.Default.IsComposeSupported;
        }

        public Task SendEmail(string? subject, string? body, List<string>? recipients)
        {
#if WINDOWS
            var uri = this.CreateMailToUri(subject, body, recipients);
            return Launcher.TryOpenAsync(uri);
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
    }
}
