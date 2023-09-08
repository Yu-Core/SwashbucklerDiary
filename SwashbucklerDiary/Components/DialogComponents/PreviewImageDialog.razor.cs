using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary.Components
{
    public partial class PreviewImageDialog : DialogComponentBase, IAsyncDisposable
    {
        private IJSObjectReference module = default!;

        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public override bool Value
        {
            get => base.Value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Src { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/zoom-helper.js");
                await module.InvokeVoidAsync("initZoom", "#zoom-image");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private string FilePath => GetFilePath();

        private async void SetValue(bool value)
        {
            if (base.Value == value)
            {
                return;
            }

            base.Value = value;
            if (!value && module is not null)
            {
                await module.InvokeVoidAsync("reset", "#zoom-image");
            }
        }

        private string GetFilePath()
        {
            string src = StaticCustomScheme.ReverseCustomSchemeRender(Src);
            return AppDataService.CustomSchemeUriToFilePath(src);
        }

        private async Task SaveToLocal()
        {
            var name = Path.GetFileName(FilePath);
            var path = await PlatformService.SaveFileAsync(name, FilePath);
            if (path is not null)
            {
                await AlertService.Success(I18n.T("Share.SaveSuccess"));
            }
        }

        private async Task Share()
        {
            await PlatformService.ShareFile(I18n.T("Share.Share"), FilePath);
            await HandleAchievements(AchievementType.Share);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
