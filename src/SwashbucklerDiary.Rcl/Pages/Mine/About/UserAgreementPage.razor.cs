using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UserAgreementPage : ImportantComponentBase
    {
        private string? content;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadingData();
        }

        private async Task LoadingData()
        {
            var uri = $"docs/user-agreement/{I18n.Culture}.md";
            content = await StaticWebAssets.ReadContentAsync(uri);
        }
    }
}
