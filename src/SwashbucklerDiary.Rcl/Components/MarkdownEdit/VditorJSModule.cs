using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class VditorJSModule : JSModuleExtension
    {
        public VditorJSModule(IJSRuntime js) : base(js, "js/vditor-helper.js")
        {
        }

        public async Task After()
        {
            await base.InvokeVoidAsync("after", null);
        }

        public async Task Focus(ElementReference element)
        {
            await base.InvokeVoidAsync("focus", element);
        }

        public async Task Autofocus(ElementReference element)
        {
            await base.InvokeVoidAsync("autofocus", element);
        }

        public async Task UploadFile(ElementReference element, ElementReference? inputFile)
        {
            if (inputFile is null)
            {
                return;
            }

            await base.InvokeVoidAsync("uploadFile", element, inputFile);
        }
    }
}
