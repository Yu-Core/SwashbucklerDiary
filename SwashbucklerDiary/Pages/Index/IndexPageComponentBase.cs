using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Pages
{
    public class IndexPageCompentBase : MyComponentBase
    {
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;

    }
}
