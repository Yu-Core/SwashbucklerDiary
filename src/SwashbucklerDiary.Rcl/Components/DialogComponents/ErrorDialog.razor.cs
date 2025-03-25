using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ErrorDialog
    {
        private int timeRemaining = 3;
        private bool isCountingDown = true;
        private PeriodicTimer? timer;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await StartCountdown();
            }
        }

        private string Text => isCountingDown
            ? $"{I18n.T("An abnormality occurred. It is being saved to the log. Please wait...")}{timeRemaining}s"
            : I18n.T("An abnormal situation has occurred. It has been saved to the log. Please exit the application.");

        private void Quit()
        {
            AppLifecycle.QuitApp();
        }

        private async Task StartCountdown()
        {
            isCountingDown = true;

            timer = new PeriodicTimer(TimeSpan.FromSeconds(1)); // 每秒触发一次

            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    timeRemaining--;

                    if (timeRemaining > 0)
                    {
                        StateHasChanged(); // 更新 UI
                    }
                    else
                    {
                        isCountingDown = false;
                        StateHasChanged();
                        break;
                    }
                }
            }
            finally
            {
                timer?.Dispose(); // 确保定时器被释放
            }
        }
    }
}