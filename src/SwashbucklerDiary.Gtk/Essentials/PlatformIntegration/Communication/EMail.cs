using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> SendEmail(string? subject, string? body, List<string>? recipients)
        {
            var uriString = EmailHelper.CreateMailToUri(subject, body, recipients);
            return OpenLauncher(uriString);
        }
    }
}
