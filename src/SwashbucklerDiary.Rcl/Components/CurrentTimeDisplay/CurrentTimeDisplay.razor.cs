using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class CurrentTimeDisplay : IDisposable
    {
        private DateTime currentTime = DateTime.Now;
        private PeriodicTimer? timer;
        private CancellationTokenSource? cts;

        [Parameter]
        public string? Format { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _ = UpdateTimeAsync(); // 启动后台更新任务
        }

        private async Task UpdateTimeAsync()
        {
            cts = new CancellationTokenSource();
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            try
            {
                while (await timer.WaitForNextTickAsync(cts.Token))
                {
                    currentTime = DateTime.Now;
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，无需处理
            }
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