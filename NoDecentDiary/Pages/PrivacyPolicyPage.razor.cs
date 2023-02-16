using Microsoft.AspNetCore.Components;
using NoDecentDiary.Components;
using NoDecentDiary.Shared;
using System;

namespace NoDecentDiary.Pages
{
    public partial class PrivacyPolicyPage : PageComponentBase
    {
        private string? Content { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadingData();
            await base.OnInitializedAsync();
        }

        private async Task LoadingData()
        {
            var uri = I18n.T("FilePath.PrivacyPolicy");
            Content = await SystemService.ReadMarkdown(uri);
        }
    }
}
