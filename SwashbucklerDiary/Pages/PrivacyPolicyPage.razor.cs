using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Shared;
using System;

namespace SwashbucklerDiary.Pages
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
            var uri = I18n.T("FilePath.PrivacyPolicy")!;
            Content = await SystemService.ReadMarkdownFile(uri);
        }
    }
}
