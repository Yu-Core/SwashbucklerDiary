using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        private bool isInitialized;

        private ElementReference _previewImg;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private ZoomJSModule Module { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Parameter]
        public string? Src { get; set; }

        protected async Task BeforeShowContent()
        {
            if (!isInitialized)
            {
                await Module.Init(_previewImg);
                isInitialized = true;
            }
        }

        protected override async Task InternalVisibleChanged(bool value)
        {
            await Module.Reset(_previewImg);

            await base.InternalVisibleChanged(value);
        }

        private async Task HandleReset()
        {
            await Module.Reset(_previewImg);
        }

        private async Task SaveToLocal()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await PopupServiceHelper.Error(I18n.T("Image.Not exist"));
                return;
            }

            await MediaResourceManager.SaveImageAsync(Src);
        }

        private async Task Share()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await PopupServiceHelper.Error(I18n.T("Image.Not exist"));
                return;
            }

            var isSuccess = await MediaResourceManager.ShareImageAsync(I18n.T("Share.Share"), Src);
            if (isSuccess)
            {
                await HandleAchievements(Achievement.Share);
            }
        }
    }
}
