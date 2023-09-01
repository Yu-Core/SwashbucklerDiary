using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class PageComponentBase : MyComponentBase, IDisposable
    {
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;
        
        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void NavigateToBack()
        {
            NavigateService.NavigateToBack();
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
        }
    }
}
