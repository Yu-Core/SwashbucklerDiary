using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class PreviewImageDialogJSModule : CustomJSModule
    {
        public PreviewImageDialogJSModule(IJSRuntime js) : base(js, "Components/DialogComponents/PreviewImageDialog/PreviewImageDialog.razor.js")
        {
        }

        public async Task Init(ElementReference elementReference)
        {
            await base.InvokeVoidAsync("init", elementReference);
        }

        public async Task Reset(ElementReference elementReference)
        {
            await base.InvokeVoidAsync("reset", elementReference);
        }
    }
}
