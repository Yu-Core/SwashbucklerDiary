using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class CurrentTimeDisplay : IDisposable
    {
        private string? currentTimeString;
        private PeriodicTimer? timer;
        private CancellationTokenSource? cts;

        [Parameter]
        public string? Format { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            StartUpdateTime(); // 启动后台更新任务
        }

        private void StartUpdateTime()
        {
            cts = new CancellationTokenSource();
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            Task.Run(async () =>
            {
                try
                {
                    while (await timer.WaitForNextTickAsync(cts.Token))
                    {
                        currentTimeString = DateTime.Now.ToString(Format);
                        await InvokeAsync(StateHasChanged);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常取消，无需处理
                }
            });
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            timer?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}