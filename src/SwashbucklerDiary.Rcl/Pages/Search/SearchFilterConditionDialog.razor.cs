using DeepCloner.Core;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchFilterConditionDialog
    {
        private SearchFilterConditionForm internalValue = new();

        [Parameter]
        public SearchFilterConditionForm Value { get; set; } = new();

        [Parameter]
        public EventCallback<SearchFilterConditionForm> OnOk { get; set; }

        private void BeforeShowContent()
        {
            internalValue = Value.DeepClone();
        }

        private async Task HandleOnOK()
        {
            if (OnOk.HasDelegate)
            {
                await OnOk.InvokeAsync(internalValue);
            }
        }
    }
}