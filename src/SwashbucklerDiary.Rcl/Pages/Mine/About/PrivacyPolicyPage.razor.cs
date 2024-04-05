using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyPolicyPage : ImportantComponentBase
    {
        private string? content;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadingData();
        }

        private async Task LoadingData()
        {
            var uri = $"docs/privacy-policy/{I18n.Culture}.md";
            content = await StaticWebAssets.ReadContentAsync(uri);
        }
    }
}
