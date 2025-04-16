namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAlertService
    {
        Task EnqueueSnackbarAsync(string? message);

        Task EnqueueSnackbarAsync(string? title, string? message);

        Task Success(string? message);

        Task Success(string? title, string? message);

        Task Error(string? message);

        Task Error(string? title, string? message);

        Task Info(string? message);

        Task Info(string? title, string? message);

        Task Warning(string? message);

        Task Warning(string? title, string? message);

        Task StartLoading(bool opacity = true);

        Task StopLoading();
    }
}
