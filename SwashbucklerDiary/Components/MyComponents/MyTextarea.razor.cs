using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public partial class MyTextarea : IAsyncDisposable
    {
        private IJSObjectReference module = default!;

        private MTextarea MTextarea = default!;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Class { get; set; }

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        public async Task InsertValueAsync(string value)
        {
            if (module == null)
            {
                return;
            }

            Value = await module.InvokeAsync<string>("insertText", new object[2] { MTextarea.InputElement, value });
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        protected override void OnInitialized()
        {
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/mmtextarea-helper.js");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;

        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
