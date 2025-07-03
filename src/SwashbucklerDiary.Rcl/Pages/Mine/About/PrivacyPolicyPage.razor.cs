using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Extensions;

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
            content = await StaticWebAssets.ReadI18nContentAsync("docs/privacy-policy/{0}.md", I18n.Culture);
        }
    }
}
