using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class FilterConditionChipGroup
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public FilterCondition Value { get; set; }

        [Parameter]
        public EventCallback<FilterCondition> ValueChanged { get; set; }

        private StringNumber InternalValue
        {
            get => (int)Value;
            set => Value = (FilterCondition)value.ToInt32();
        }

        private async Task InternalValueChanged(StringNumber value)
        {
            InternalValue = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }
    }
}