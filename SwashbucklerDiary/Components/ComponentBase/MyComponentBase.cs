using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public abstract class MyComponentBase : ComponentBase
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;
        [Inject]
        protected II18nService I18n { get; set; } = default!;
        [Inject]
        protected IAlertService AlertService { get; set; } = default!;
        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        protected NavigationManager Navigation => NavigateService.Navigation!;
        protected bool Dark => ThemeService.Dark;
        protected bool Light => ThemeService.Light;

        protected Task ToDo()
        {
            return AlertService.Info(I18n.T("ToDo.Title"), I18n.T("ToDo.Content"));
        }

        protected void To(string url)
        {
            NavigateService.NavigateTo(url);
        }

        /// <summary>
        /// CreateEventCallback
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected EventCallback EC(Func<Task> callback)
        {
            return EventCallback.Factory.Create(this, callback);
        }

        /// <summary>
        /// CreateEventCallback
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected EventCallback EC(Action callback)
        {
            return EventCallback.Factory.Create(this, callback);
        }
    }
}
