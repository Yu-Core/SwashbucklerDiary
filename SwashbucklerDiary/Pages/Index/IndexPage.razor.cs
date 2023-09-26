using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Pages
{
    public partial class IndexPage : DiariesPageComponentBase
    {
        private IndexDiary IndexDiary = default!;
        private IndexHistory IndexHistory = default!;
        private IndexMine IndexMine = default!;
        private StringNumber NavigationIndex
        {
            get => MainLayoutOptions.NavigationIndex;
            set => MainLayoutOptions.NavigationIndex = value;
        }
        private List<NavigationButton> NavigationButtons => MainLayoutOptions.NavigationButtons;

        [Inject]
        private IStateService StateService { get; set; } = default!;

        [CascadingParameter]
        public MainLayoutOptions MainLayoutOptions { get; set; } = default!;

        protected override void OnInitialized()
        {
            FirstLauch();
            NavigateService.SetRootPath();
            MainLayoutOptions.NavigationIndexAction += NavigationIndexChanged;
            base.OnInitialized();
        }

        protected override void OnDispose()
        {
            StateService.FirstLauch -= FirstLauchUpdateDiaries;
            MainLayoutOptions.NavigationIndexAction -= NavigationIndexChanged;
            base.OnDispose();
        }

        protected override async Task OnResume()
        {
            await LoadSettings();
            await base.OnResume();
        }

        private void FirstLauch()
        {
            StateService.FirstLauch += FirstLauchUpdateDiaries;
        }

        private async Task FirstLauchUpdateDiaries()
        {
            await UpdateDiariesAsync();
        }

        private void NavigationIndexChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        private async Task LoadSettings()
        {
            await IndexDiary.LoadSettings();
            await IndexMine.LoadSettings();
            await IndexMine.SetAvatar();
        }
    }
}
