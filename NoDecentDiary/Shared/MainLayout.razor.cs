using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject]
        private MasaBlazor? MasaBlazor { get; set; }
        [Inject]
        private NavigationManager? Navigation { get; set; }

        StringNumber SelectedItem = 0;

        readonly List<NavigationButton> NavigationButtons = new()
        {
            new NavigationButton(0,"日记","mdi-notebook-outline","mdi-notebook","Diaries"),
            new NavigationButton(1,"回忆","mdi-clock-outline","mdi-clock","History"),
            new NavigationButton(2,"我的","mdi-account-outline","mdi-account","Mine")
        };

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

        protected override Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            return base.OnInitializedAsync();
        }

        private string? GetIcon(NavigationButton navigationButton)
        {
            return SelectedItem == navigationButton.Id ? navigationButton.SelectIcon : navigationButton.Icon;
        }

        private void ChangeView(NavigationButton navigationButton)
        {
            Navigation!.NavigateTo(navigationButton.Href);
        }

        private bool ShowBottomNavigation()
        {
            var url = Navigation!.ToBaseRelativePath(Navigation.Uri);
            return NavigationButtons.Any(it => it.Href == url.Split("?")[0]);
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }
    }
}
