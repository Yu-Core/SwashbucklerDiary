using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class NumberLock
    {
        private bool shake;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public int Length { get; set; } = 4;

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public bool ExtraButton { get; set; }

        [Parameter]
        public string? ExtraButtonIconName { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> ExtraButtonOnClick { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public virtual IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<NumberLockFinishArguments> OnFinish { get; set; }

        private void PressNumber(int num)
        {
            if (Value is null || Value.Length < 4)
            {
                Value += num;
                if (ValueChanged.HasDelegate)
                {
                    ValueChanged.InvokeAsync(Value);
                }

                if (Value.Length == Length)
                {
                    Task.Delay(200).ContinueWith(_ => InvokeAsync(CheckAsync));
                }
            }
        }

        private async Task CheckAsync()
        {
            NumberLockFinishArguments args = new()
            {
                Value = Value
            };
            await OnFinish.InvokeAsync(args);
            if (args.IsFail)
            {
                shake = true;
                StateHasChanged();
                await Task.Delay(400);

                shake = false;
                StateHasChanged();

                await Task.Delay(100);
                Value = "";
                StateHasChanged();
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
            }
        }

        private async Task Backspace()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                Value = Value[..^1];
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
            }
        }
    }
}