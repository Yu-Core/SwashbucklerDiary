using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public async ValueTask<bool> IsMailSupported()
        {
            var module = await Module;
            return module.Invoke<bool>("isMailSupported");
        }

        public async Task SendEmail(string? subject, string? body, List<string>? recipients)
        {
            var uri = EmailHelper.CreateMailToUri(subject, body, recipients);
            await OpenLauncher(uri);
        }
    }
}
