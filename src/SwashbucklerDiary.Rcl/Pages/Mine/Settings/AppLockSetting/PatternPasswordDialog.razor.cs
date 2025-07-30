using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PatternPasswordDialog
    {
        private string? firstValue;

        private GestureUnlock? gestureUnlock;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> OnFinish { get; set; }

        private void HandleOnBeforeShowContent()
        {
            firstValue = null;
        }

        private void HandleOnAfterShowContent()
        {
            gestureUnlock?.Reset();
        }

        private async Task HandleNumberLockOnFinish(LockFinishArguments args)
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
                gestureUnlock?.Reset();

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