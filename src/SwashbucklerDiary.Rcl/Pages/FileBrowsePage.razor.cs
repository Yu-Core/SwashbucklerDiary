using Masa.Blazor;
using Masa.Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class FileBrowsePage : ImportantComponentBase, IAsyncDisposable
    {
        private bool showDelete;

        private bool showMenu;

        private bool contentLoading;

        private StringNumber tab = 0;

        private SwiperTabItems swiperTabItems = default!;

        private readonly List<string> tabNames = ["FileBrowse.Image.Name", "FileBrowse.Audio.Name", "FileBrowse.Video.Name"];

        private IJSObjectReference module = default!;

        private List<ResourceModel> imageResources = [];

        private List<ResourceModel> audioResources = [];

        private List<ResourceModel> videoResources = [];

        private List<DynamicListItem> menuItems = [];

        [Inject]
        protected IResourceService ResourceService { get; set; } = default!;

        public async ValueTask DisposeAsync()
        {
            base.OnDispose();
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            NavigateService.BeforePopToRoot += BeforePopToRoot;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("Pages/FileBrowsePage.razor.js");
                await RecordScrollInfo();
                await UpdateResourcesAsync();
                StateHasChanged();
            }
        }

        protected override void OnDispose()
        {
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            base.OnDispose();
        }

        protected override async Task OnResume()
        {
            await UpdateResourcesAsync();
            await base.OnResume();
            await RestoreScrollPosition();
        }

        protected override async void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            base.NavigationManagerOnLocationChanged(sender, e);
            if (!thisPageUrl.EqualsAbsolutePath(Navigation.Uri))
            {
                if (module is null)
                {
                    return;
                }

                await module.InvokeVoidAsync("stopRecordScrollInfo");
                contentLoading = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "FileBrowse.DeleteUnused","mdi-delete-outline", ()=>showDelete=true),
            ];
        }

        private async Task UpdateResourcesAsync()
        {
            var resources = await ResourceService.QueryAsync();
            imageResources = resources.Where(it => it.ResourceType == MediaResource.Image).ToList();
            audioResources = resources.Where(it => it.ResourceType == MediaResource.Audio).ToList();
            videoResources = resources.Where(it => it.ResourceType == MediaResource.Video).ToList();
        }

        private async Task DeleteUnusedResources()
        {
            showDelete = false;
            StateHasChanged();
            var flag = await ResourceService.DeleteUnusedResourcesAsync(_ => true);
            if (flag)
            {
                await UpdateResourcesAsync();
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl != args.NextUri) return;

            if (thisPageUrl != args.PreviousUri) return;

            if (swiperTabItems.ActiveItem is null) return;

            await JS.ScrollTo($"#{swiperTabItems.ActiveItem.Id}", 0);
        }

        private async Task RecordScrollInfo()
        {
            if (module is null)
            {
                return;
            }

            await module.InvokeVoidAsync("recordScrollInfo", swiperTabItems.ChildTabItems.Select(it => $"#{it.Id}"));
        }

        private async Task RestoreScrollPosition()
        {
            await Task.Delay(300);
            await module.InvokeVoidAsync("restoreScrollPosition", swiperTabItems.ChildTabItems.Select(it => $"#{it.Id}"));
            contentLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
