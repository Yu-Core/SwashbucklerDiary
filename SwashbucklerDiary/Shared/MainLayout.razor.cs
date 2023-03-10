using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Globalization;

namespace SwashbucklerDiary.Shared
{
    public partial class MainLayout : IDisposable
    {
        StringNumber SelectedItemIndex = 0;
        List<NavigationButton> NavigationButtons = new();

        [Inject]
        MasaBlazor MasaBlazor { get; set; } = default!;
        [Inject]
        NavigationManager Navigation { get; set; } = default!;
        [Inject]
        INavigateService NavigateService { get; set; } = default!;
        [Inject]
        I18n I18n { get; set; } = default!;
        [Inject]
        ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        IPopupService PopupService { get; set; } = default!;
        [Inject]
        IAlertService AlertService { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await LoadSettings();
            NavigateService.Initialize(Navigation);
            AlertService.Initialize(PopupService);
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            await base.OnInitializedAsync();
        }

        private class NavigationButton : ViewListItem
        {
            public string? SelectedIcon { get; set; }

            public NavigationButton(string text, string icon, string selectedIcon, Action action) : base(text, icon, action)
            {
                SelectedIcon = selectedIcon;
            }
        }

        private async Task LoadSettings()
        {
            var flag = await SettingsService!.ContainsKey("Language");
            if (flag)
            {
                var language = await SettingsService!.GetLanguage();
                I18n.SetCulture(new CultureInfo(language));
            }
        }

        private void LoadView()
        {
            NavigationButtons = new()
            {
                new ( "Main.Diary", "mdi-notebook-outline", "mdi-notebook", ()=>To("")),
                new ( "Main.History", "mdi-clock-outline", "mdi-clock", ()=>To("history")),
                new ( "Main.Mine", "mdi-account-outline", "mdi-account", ()=>To("mine"))
            };
        }

        private bool ShowBottomNavigation
        {
            get
            {
                if (MasaBlazor.Breakpoint.SmAndUp)
                {
                    return false;
                }
                string[] links = { "", "history", "mine" };
                var url = Navigation!.ToBaseRelativePath(Navigation.Uri);
                return links.Any(it => it == url.Split("?")[0]);
            }
        }

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        protected void To(string url)
        {
            Navigation.NavigateTo(url);
        }
    }
}
