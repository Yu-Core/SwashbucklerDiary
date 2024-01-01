using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly
{
    public partial class App
    {
        [Inject]
        private SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await SystemThemeJSModule.InitializedAsync();
            }
        }
    }
}
