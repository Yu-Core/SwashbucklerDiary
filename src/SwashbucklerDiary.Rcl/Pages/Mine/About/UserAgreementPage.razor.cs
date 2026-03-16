using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UserAgreementPage : ImportantComponentBase
    {
        private string? content;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            content = await StaticWebAssets.ReadRclI18nTextAsync("docs/user-agreement/{0}.md", I18n.Culture);
        }
    }
}
