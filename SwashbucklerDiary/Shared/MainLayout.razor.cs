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
        StringNumber SelectedItem = 0;
        readonly List<NavigationButton> NavigationButtons = new()
        {
            new NavigationButton(0,"Main.Diary","mdi-notebook-outline","mdi-notebook",""),
            new NavigationButton(1,"Main.History","mdi-clock-outline","mdi-clock","History"),
            new NavigationButton(2,"Main.Mine","mdi-account-outline","mdi-account","Mine")
        };

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        public INavigateService NavigateService { get; set; } = default!;
        [Inject]
        private I18n I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private ISystemService SystemService { get; set; } = default!;
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await FirstLaunch();
            await LoadSettings();
            NavigateService.Navigation = Navigation;
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            await base.OnInitializedAsync();
        }

        private class NavigationButton
        {
            public NavigationButton(int id, string title, string icon, string selectIcon, string href)
            {
                Id = id;
                Title = title;
                Icon = icon;
                SelectIcon = selectIcon;
                Href = href;
            }
            public int Id;
            public string Title { get; set; }
            public string Icon { get; set; }
            public string SelectIcon { get; set; }
            public string Href { get; set; }
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

        private bool ShowBottomNavigation
        {
            get
            {
                if (MasaBlazor.Breakpoint.SmAndUp)
                {
                    return true;
                }
                var url = Navigation!.ToBaseRelativePath(Navigation.Uri);
                return NavigationButtons.Any(it => it.Href == url.Split("?")[0]);
            }
        }

        private string? GetIcon(NavigationButton navigationButton)
        {
            return SelectedItem == navigationButton.Id ? navigationButton.SelectIcon : navigationButton.Icon;
        }

        private void ChangeView(NavigationButton navigationButton)
        {
            Navigation.NavigateTo(navigationButton.Href);
        }

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        private async Task FirstLaunch()
        {
            bool flag = SystemService.IsFirstLaunch();
            if (flag)
            {
                var uri = I18n.T("FilePath.FunctionalDescription")!;
                string content = await SystemService.ReadMarkdown(uri);
                var diary = new DiaryModel()
                {
                    Title = I18n.T("FunctionalDescription"),
                    Content = content,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                };
                await DiaryService.AddAsync(diary);
            }
        }
    }
}
