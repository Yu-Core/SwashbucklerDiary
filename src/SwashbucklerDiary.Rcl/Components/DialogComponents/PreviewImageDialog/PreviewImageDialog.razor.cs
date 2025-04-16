using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        private readonly string id = $"zoom-{Guid.NewGuid()}";

        private bool isInitialized;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private ZoomJSModule Module { get; set; } = default!;

        [Parameter]
        public string? Src { get; set; }

        protected async Task BeforeShowContent()
        {
            if (!isInitialized)
            {
                await Module.Init($"#{id}");
                isInitialized = true;
            }
        }

        protected override async Task InternalVisibleChanged(bool value)
        {
            await base.InternalVisibleChanged(value);
            await Module.Reset($"#{id}");
        }

        private async Task SaveToLocal()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await AlertService.Error(I18n.T("File does not exist"));
                return;
            }

            await MediaResourceManager.SaveFileAsync(Src);
        }

        private async Task Share()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await AlertService.Error(I18n.T("File does not exist"));
                return;
            }

            var isSuccess = await MediaResourceManager.ShareImageAsync(I18n.T("Share"), Src);
            if (isSuccess)
            {
                await HandleAchievements(Achievement.Share);
            }
        }
    }
}
