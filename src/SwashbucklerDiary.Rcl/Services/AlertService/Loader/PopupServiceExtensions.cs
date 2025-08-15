using Masa.Blazor;

namespace SwashbucklerDiary.Rcl.Services
{
    public static class PopupServiceExtensions
    {
        public static void ShowLoader(this IPopupService service, Action<LoaderOptions>? options = null)
        {
            LoaderOptions param = new();
            options?.Invoke(param);

            var parameters = param.ToParameters();

            service.Open(typeof(Loader), parameters);
        }

        public static void HideLoader(this IPopupService service)
        {
            service.Close(typeof(Loader));
        }
    }
}
