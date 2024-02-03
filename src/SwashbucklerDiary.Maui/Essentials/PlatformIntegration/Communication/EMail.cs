#if WINDOWS
using SwashbucklerDiary.Rcl.Essentials;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<bool> SendEmail(string? subject, string? body, List<string>? recipients)
        {
            if (!Email.Default.IsComposeSupported)
            {
                return false;
            }
#if WINDOWS
            var uri = EmailHelper.CreateMailToUri(subject, body, recipients);
            return await Launcher.TryOpenAsync(uri);
#else
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients
            };
            await Email.Default.ComposeAsync(message);
            return true;
#endif
        }
    }
}
