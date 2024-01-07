using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyPolicyPage : ImportantComponentBase
    {
        private string? content { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadingData();
        }

        private async Task LoadingData()
        {
            var uri = I18n.T("FilePath.PrivacyPolicy")!;
            content = await StaticWebAssets.ReadContentAsync(uri);
        }
    }
}
