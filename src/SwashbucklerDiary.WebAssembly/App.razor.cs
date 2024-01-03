using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly
{
    public partial class App
    {
        [Inject]
        private SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            I18n.OnChanged += StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await SystemThemeJSModule.InitializedAsync();
                await ((AppLifecycle)AppLifecycle).InitializedAsync();
            }
        }
    }
}
