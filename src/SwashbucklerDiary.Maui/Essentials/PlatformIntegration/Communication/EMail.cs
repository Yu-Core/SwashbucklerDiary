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
            var uri = PlatformIntegrationHelper.CreateMailToUri(subject, body, recipients);
            return await Launcher.TryOpenAsync(uri).ConfigureAwait(false);
#else
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients
            };
            await Email.Default.ComposeAsync(message).ConfigureAwait(false);
            return true;
#endif
        }
    }
}
