using Masa.Blazor;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public class MTextareaExtension : MTextarea,IAsyncDisposable
    {
        private IJSObjectReference module = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await Js.InvokeAsync<IJSObjectReference>("import", "./js/mmtextarea-helper.js");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task InsertValueAsync(string value)
        {
            if (module == null)
            {
                return;
            }

            Value = await module.InvokeAsync<string>("insertText", new object[2] { InputElement, value });
            if(ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            base.Dispose(disposing:true);
            GC.SuppressFinalize(this);
        }
    }
}
