namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAlertService
    {
        Task EnqueueSnackbarAsync(string? message);

        Task EnqueueSnackbarAsync(string? title, string? message);

        Task SuccessAsync(string? message);

        Task SuccessAsync(string? title, string? message);

        Task ErrorAsync(string? message);

        Task ErrorAsync(string? title, string? message);

        Task InfoAsync(string? message);

        Task InfoAsync(string? title, string? message);

        Task WarningAsync(string? message);

        Task WarningAsync(string? title, string? message);

        void StartLoading(bool opacity = true);

        void StopLoading();
    }
}
