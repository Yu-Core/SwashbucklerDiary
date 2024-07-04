using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MarkdownJSModule : JSModuleExtension
    {
        public MarkdownJSModule(IJSRuntime js) : base(js, "js/markdown-helper.js")
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
    }
}
