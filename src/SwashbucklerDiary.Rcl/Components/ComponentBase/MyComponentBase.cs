using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Runtime.CompilerServices;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MyComponentBase : ComponentBase, IAsyncDisposable
    {
        private readonly PropertyWatcher _watcher;

        protected bool IsDisposed { get; private set; }

        public MyComponentBase()
        {
            _watcher = new PropertyWatcher(GetType());
        }

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected IPopupServiceHelper PopupServiceHelper { get; set; } = default!;

        [Inject]
        protected IAchievementService AchievementService { get; set; } = default!;

        [Inject]
        protected ISettingService SettingService { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                RegisterWatchers(_watcher);
            }
        }

        protected Task ToDo()
        {
            return PopupServiceHelper.Info(I18n.T("ToDo.Title"), I18n.T("ToDo.Content"));
        }

        protected void To(string url, bool cacheCurrentURL = true)
        {
            if (!cacheCurrentURL)
            {
                NavigateController.RemovePageCache(NavigationManager.Uri);
            }

            NavigationManager.NavigateTo(url);
        }

        protected virtual async Task HandleAchievements(Achievement type)
        {
            var messages = await AchievementService.UpdateUserState(type);
            await AlertAchievements(messages);
        }

        protected async Task AlertAchievements(List<string> messages)
        {
            bool achievementsAlert = SettingService.Get(s => s.AchievementsAlert);
            if (!achievementsAlert)
            {
                return;
            }

            foreach (var item in messages)
            {
                await PopupServiceHelper.Info(I18n.T("Achievement.AchieveAchievements"), I18n.T(item));
            }
        }

        /// <summary>
        /// Register watchers at the first render.
        /// </summary>
        /// <param name="watcher"></param>
        protected virtual void RegisterWatchers(PropertyWatcher watcher)
        {
        }

        protected TValue? GetValue<TValue>(TValue? @default = default, [CallerMemberName] string name = "")
        {
            return _watcher.GetValue(@default, name);
        }

        protected TValue? GetComputedValue<TValue>(Func<TValue> valueFactory, string[] dependencyProperties,
            [CallerMemberName] string name = "")
        {
            return _watcher.GetComputedValue(valueFactory, dependencyProperties, name);
        }

        protected void SetValue<TValue>(TValue value, [CallerMemberName] string name = "",
            bool disableIListAlwaysNotifying = false)
        {
            _watcher.SetValue(value, name, disableIListAlwaysNotifying);
        }

        protected void SetValueWithNoEffect<TValue>(TValue? value, string name)
        {
            var property = _watcher.GetOrSetProperty<TValue>(default, name);
            property?.SetValueWithNoEffect(value);
        }

        protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
            {
                return;
            }

            try
            {
                await DisposeAsyncCore().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // ignored
            }
            // HACK: remove this after https://github.com/dotnet/aspnetcore/issues/52119 is fixed
            catch (JSException e) when (e.Message.Contains("has it been disposed") && !OperatingSystem.IsBrowser())
            {
                // ignored
            }
            catch (InvalidOperationException e) when (e.Message.Contains("prerendering"))
            {
                // ignored
            }

            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }
}
