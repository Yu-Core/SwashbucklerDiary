using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : ShowContentDialogComponentBase
    {
        private readonly string id = $"B-{Guid.NewGuid()}";

        private bool isInitialized;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private ZoomJSModule Module { get; set; } = default!;

        [Parameter]
        public override bool Visible
        {
            get => base.Visible;
            set => SetVisible(value);
        }

        [Parameter]
        public string? Src { get; set; }

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();

            if (!isInitialized)
            {
                await Module.Init($"#{id}");
                isInitialized = true;
            }
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
                await Module.Reset($"#{id}");
            }
        }

        private async Task SaveToLocal()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await AlertService.Error(I18n.T("Image.Not exist"));
                return;
            }

            var isSuccess = await MediaResourceManager.SaveImageAsync(Src);
            if (isSuccess)
            {
                await AlertService.Success(I18n.T("Share.SaveSuccess"));
            }
        }

        private async Task Share()
        {
            if (string.IsNullOrEmpty(Src))
            {
                await AlertService.Error(I18n.T("Image.Not exist"));
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
