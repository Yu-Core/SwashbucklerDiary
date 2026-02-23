using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> SendEmail(string? subject, string? body, List<string>? recipients)
        {
            var uri = PlatformIntegrationHelper.CreateMailToUri(subject, body, recipients);
            return OpenLauncher(uri);
        }
    }
}
