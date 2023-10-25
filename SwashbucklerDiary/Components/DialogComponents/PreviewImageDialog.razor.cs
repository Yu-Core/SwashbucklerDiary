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

        private readonly string Id = $"zoom-image-{Guid.NewGuid()}";

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
                await module.InvokeVoidAsync("initZoom", $"#{Id}");
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
                await module.InvokeVoidAsync("reset", $"#{Id}");
            }
        }

        private string GetFilePath()
        {
            if(string.IsNullOrEmpty(Src))
            {
                return string.Empty;
            }

            var src = Src;
            if (!src.StartsWith(StaticCustomPath.CustomPathPrefix))
            {
                return string.Empty;
            }

            return AppDataService.CustomPathUriToFilePath(src);
        }

        private async Task SaveToLocal()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                await AlertService.Error(I18n.T("Image.Not exist"));
                return;
            }

            var name = Path.GetFileName(FilePath);
            var path = await PlatformService.SaveFileAsync(name, FilePath);
            if (path is not null)
            {
                await AlertService.Success(I18n.T("Share.SaveSuccess"));
            }
        }

        private async Task Share()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                await AlertService.Error(I18n.T("Image.Not exist"));
                return;
            }

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
