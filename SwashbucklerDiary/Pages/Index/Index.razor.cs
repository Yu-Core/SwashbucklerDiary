using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Pages
{
    public partial class Index : DiariesPageComponentBase
    {
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

        private void FirstLauch()
        {
            StateService.FirstLauch += FirstLauchUpdateDiaries;
        }

        private async Task FirstLauchUpdateDiaries()
        {
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigationIndexChanged()
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
