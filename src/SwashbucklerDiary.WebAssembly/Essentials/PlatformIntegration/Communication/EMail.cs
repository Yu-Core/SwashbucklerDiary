using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> SendEmail(string? subject, string? body, List<string>? recipients)
        {
            var uri = EmailHelper.CreateMailToUri(subject, body, recipients);
            return OpenLauncher(uri);
        }
    }
}
