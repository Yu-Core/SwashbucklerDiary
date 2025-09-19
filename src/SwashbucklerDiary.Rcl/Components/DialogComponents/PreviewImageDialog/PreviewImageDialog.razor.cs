using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        private bool isInitialized;

        private string? _previousSrc;

        private ElementReference elementReference;

        private MediaResourcePath? mediaResourcePath;

        private PreviewImageDialogJSModule? jSModule;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Parameter]
        public string? Src { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (_previousSrc != Src)
            {
                _previousSrc = Src;
                mediaResourcePath = MediaResourceManager.ToMediaResourcePath(NavigationManager, Src);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (!IsDisposed && firstRender)
            {
                jSModule = new(JS);
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            await jSModule.TryDisposeAsync();
        }

        protected async Task BeforeShowContent()
        {
            if (!isInitialized && jSModule is not null)
            {
                await jSModule.Init(elementReference);
                isInitialized = true;
            }
        }

        protected override async Task InternalVisibleChanged(bool value)
        {
            await base.InternalVisibleChanged(value);
            if (!value && jSModule is not null)
            {
                await jSModule.Reset(elementReference);
            }
        }

        private async Task SaveToLocal()
        {
            string? filePath = await GetFilePathAsync();

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            await PlatformIntegration.SaveFileAsync(filePath);
        }

        private async Task Share()
        {
            string? filePath = await GetFilePathAsync();

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            await PlatformIntegration.ShareFileAsync(I18n.T("Share"), filePath);
            _ = HandleAchievements(Achievement.Share);
        }

        async Task<string?> GetFilePathAsync()
        {
            string? filePath = null;

            AlertService.StartLoading();
            try
            {
                filePath = await MediaResourceManager.ToFilePathAsync(mediaResourcePath);
            }
            finally
            {
                AlertService.StopLoading();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                await AlertService.ErrorAsync(I18n.T("File does not exist"));
            }

            return filePath;
        }
    }
}
