using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class ImportantComponentBase : MyComponentBase, IDisposable
    {
        private string? Url;
        
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsCurrentPage => Url is null || EqualsAbsolutePath(Navigation.Uri, Url);


        protected override void OnInitialized()
        {
            InitializedUrl();
            NavigateService.Poped += Poped;
            if(IsRootPath)
            {
                NavigateService.PopedToRoot += Poped;
            }

            base.OnInitialized();
        }

        protected virtual void NavigateToBack()
        {
            NavigateService.PopAsync();
        }

        protected Func<bool, Task> SettingChange(SettingType type)
        {
            return (bool value) => SettingsService.Save(type, value);
        }

        protected string? MSwitchTrackColor(bool value)
        {
            return value && Light ? "black" : null;
        }

        protected virtual void OnDispose()
        {
            NavigateService.Poped -= Poped;
            if (IsRootPath)
            {
                NavigateService.PopedToRoot -= Poped;
            }
        }

        protected virtual void OnResume()
        {
            InvokeAsync(StateHasChanged);
        }

        protected void Poped(PopEventArgs e)
        {
            if (EqualsAbsolutePath(e.PreviousUri, Url))
            {
                Task.Run(OnResume);
            }
        }

        private bool IsRootPath => Url == NavigateService.RootPath;

        private void InitializedUrl()
        {
            Url = Navigation.Uri;
        }

        private bool EqualsAbsolutePath(string? uri1, string? uri2)
        {
            if (uri1 == null || uri2 == null)
            {
                return false;
            }

            return new Uri(uri1).AbsolutePath == new Uri(uri2).AbsolutePath;
        }
    }
}
