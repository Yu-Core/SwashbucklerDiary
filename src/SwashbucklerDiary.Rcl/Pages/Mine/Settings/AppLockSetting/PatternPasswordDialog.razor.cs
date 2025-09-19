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

        [Parameter]
        public Func<string, bool>? OnValidate { get; set; }

        private void HandleOnBeforeShowContent()
        {
            firstValue = null;
        }

        private async Task HandleOnAfterShowContent()
        {
            if (gestureUnlock is null) return;

            await gestureUnlock.Reset();
        }

        private async Task HandleOnFinish(LockFinishArguments args)
        {
            // 验证密码
            if (!string.IsNullOrEmpty(Value))
            {
                if (OnValidate is not null && !OnValidate.Invoke(args.Value))
                {
                    args.IsFail = true;
                    return;
                }

                if (OnFinish.HasDelegate)
                {
                    await OnFinish.InvokeAsync(args.Value);
                }

                return;
            }

            // 设置密码
            if (string.IsNullOrEmpty(firstValue))
            {
                firstValue = args.Value;
                if (gestureUnlock is not null)
                {
                    await gestureUnlock.Reset();
                }

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