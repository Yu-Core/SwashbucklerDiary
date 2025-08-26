namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VideoResourceCard : MediaResourceComponentBase
    {
        private string? src;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Display the first frame
            if (!string.IsNullOrEmpty(mediaResourcePath?.DisPlayedUrl)
                && !mediaResourcePath.DisPlayedUrl.Contains('#'))
            {
                src = mediaResourcePath.DisPlayedUrl + "#t=0.1";
            }
        }
    }
}