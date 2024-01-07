using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        private readonly string id = $"zoom-image-{Guid.NewGuid()}";

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await Module.Init($"#{id}");
            }
        }

        private async void SetVisible(bool value)
        {
            if (base.Visible == value)
            {
                return;
            }

            base.Visible = value;
            if (!value && Module is not null)
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
            if(isSuccess)
            {
                await HandleAchievements(Achievement.Share);
            }
        }
    }
}
