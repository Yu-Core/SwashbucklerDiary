#if WINDOWS
using SwashbucklerDiary.Rcl.Essentials;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public ValueTask<bool> IsMailSupported()
        {
            return ValueTask.FromResult(Email.Default.IsComposeSupported);
        }

        public Task SendEmail(string? subject, string? body, List<string>? recipients)
        {
#if WINDOWS
            var uri = EmailHelper.CreateMailToUri(subject, body, recipients);
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
