using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        private readonly string id = $"zoom-{Guid.NewGuid()}";

        private bool isInitialized;

        private ElementReference _previewImg;
        private double _zoomOutTimes = 1;
        private string _left = "50%";
        private string _top = "50%";

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private ZoomJSModule Module { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Parameter]
        public override bool Visible
        {
            get => base.Visible;
            set => SetVisible(value);
        }

        [Parameter]
        public string? Src { get; set; }

        protected async Task BeforeShowContent()
        {
            if (!isInitialized)
            {
                await Module.Init($"#{id}");
                isInitialized = true;
            }

            await Module.ImgDragAndDrop(_previewImg);
        }

        private async void SetVisible(bool value)
        {
            if (base.Visible == value)
            {
                return;
            }

            base.Visible = value;
            if (!value && Module is not null && isInitialized)
            {
                // Cannot be placed in BeforeShowContent, Because you will see the Reset animation
                await Module.Reset($"#{id}");
            }

            _zoomOutTimes = 1;
            _left = "50%";
            _top = "50%";
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

        private async Task WeelHandZoom(WheelEventArgs wheelEventArgs)
        {
            if (PlatformIntegration.CurrentPlatform == AppDevicePlatform.Android || PlatformIntegration.CurrentPlatform == AppDevicePlatform.iOS) return;

            _left = await Module.GetStyle(_previewImg, "left") ?? string.Empty;
            _top = await Module.GetStyle(_previewImg, "top") ?? string.Empty;

            if (wheelEventArgs.DeltaY < 0)
            {
                _zoomOutTimes += 0.1;
            }
            else if (_zoomOutTimes > 0.5)
            {
                _zoomOutTimes -= 0.1;
            }
        }
    }
}
