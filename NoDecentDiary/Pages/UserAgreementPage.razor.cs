using Microsoft.AspNetCore.Components;
using NoDecentDiary.Components;
using NoDecentDiary.Shared;

namespace NoDecentDiary.Pages
{
    public partial class UserAgreementPage : PageComponentBase
    {
        private string? Content { get; set; }

        [CascadingParameter]
        public Error Error { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadingData();
            await base.OnInitializedAsync();
        }

        private async Task LoadingData()
        {
            try
            {
                var uri = I18n.T("FilePath.UserAgreement");
                Content = await SystemService.ReadMarkdown(uri);
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }
    }
}
