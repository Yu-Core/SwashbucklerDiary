using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TextareaEdit : IDisposable
    {
        private MTextarea MTextarea = default!;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private TextareaJSModule Module { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Class { get; set; }

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        public async Task InsertValueAsync(string value)
        {
            Value = await Module.InsertText(MTextarea.InputElement, value);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;

        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
