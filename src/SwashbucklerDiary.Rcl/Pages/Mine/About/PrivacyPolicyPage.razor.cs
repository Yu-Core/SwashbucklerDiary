using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyPolicyPage : ImportantComponentBase
    {
        private string? content;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var path = $"docs/privacy-policy/{I18n.Culture}.md";
            content = await StaticWebAssets.ReadContentAsync(path);
        }
    }
}
