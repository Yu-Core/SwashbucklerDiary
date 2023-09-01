using Masa.Blazor;

namespace SwashbucklerDiary.IServices
{
    public interface IAlertService
    {
        void Initialize(object popupService);
        void SetTimeout(int timeout);
        Task Alert(string? message);
        Task Alert(string? title, string? message);
        Task Success(string? message);
        Task Success(string? title, string? message);
        Task Error(string? message);
        Task Error(string? title, string? message);
        Task Info(string? message);
        Task Info(string? title , string? message);
        Task Warning(string? message);
        Task Warning(string? title, string? message);
        Task StartLoading(bool opacity = true);
        Task StopLoading();
    }
}
