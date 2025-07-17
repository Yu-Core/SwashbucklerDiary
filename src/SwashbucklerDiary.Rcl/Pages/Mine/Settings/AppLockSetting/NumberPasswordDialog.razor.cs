using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class NumberPasswordDialog : DialogComponentBase
    {
        private string? numberLockValue;

        private string? firstValue;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> OnFinish { get; set; }

        private void HandleOnBeforeShowContent()
        {
            numberLockValue = null;
            firstValue = null;
        }

        private async Task HandleNumberLockOnFinish(NumberLockFinishArguments args)
        {
            // 验证密码
            if (!string.IsNullOrEmpty(Value))
            {
                if (Value != args.Value)
                {
                    args.IsFail = true;
                    return;
                }

                if (OnFinish.HasDelegate)
                {
                    await OnFinish.InvokeAsync(Value);
                }

                return;
            }

            // 设置密码
            if (string.IsNullOrEmpty(firstValue))
            {
                firstValue = args.Value;
                numberLockValue = null;
                return;
            }

            if (firstValue != args.Value)
            {
                args.IsFail = true;
                return;
            }

            if (OnFinish.HasDelegate)
            {
                await OnFinish.InvokeAsync(args.Value);
            }
        }
    }
}