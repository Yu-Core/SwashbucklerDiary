namespace SwashbucklerDiary.Rcl.Essentials
{
    public static partial class PlatformIntegrationHelper
    {
        public static string CreateMailToUri(string? subject, string? body, List<string>? recipients)
        {
            string emailAddress = string.Empty;
            if (recipients is not null)
            {
                emailAddress = string.Join(";", recipients);
            }

            return $"mailto:{emailAddress}?subject={subject}&body={body}";
        }
    }
}
