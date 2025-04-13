using Masa.Blazor;
using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TextareaEdit
    {
        private MTextarea mTextarea = default!;

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

        [Parameter]
        public bool Autofocus { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        private string InternalClass => new CssBuilder()
            .Add(Class)
            .Add("user-select")
            .Add("flex-grow-1")
            .Add("flex-column")
            .Build();

        public async Task InsertValueAsync(string value)
        {
            Value = await Module.InsertText(mTextarea.InputElement, value);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }
    }
}
