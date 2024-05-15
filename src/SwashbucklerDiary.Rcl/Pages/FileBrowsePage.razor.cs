using BlazorComponent;
using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class FileBrowsePage : ImportantComponentBase
    {
        private bool showDelete;

        private bool showMenu;

        private bool contentLoading;

        private StringNumber tab = 0;

        private SwiperTabItems swiperTabItems = default!;

        private readonly List<string> tabNames = ["FileBrowse.Image.Name", "FileBrowse.Audio.Name", "FileBrowse.Video.Name"];

        private List<double> scrollTops = [];

        private List<ResourceModel> imageResources = [];

        private List<ResourceModel> audioResources = [];

        private List<ResourceModel> videoResources = [];

        private List<DynamicListItem> menuItems = [];

        [Inject]
        protected IResourceService ResourceService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            NavigateService.BeforePopToRoot += BeforePopToRoot;
            NavigateService.BeforePush += BeforePush;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateResourcesAsync();
                StateHasChanged();
            }
        }

        protected override void OnDispose()
        {
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            NavigateService.BeforePush -= BeforePush;
            base.OnDispose();
        }

        protected override async Task OnResume()
        {
            await UpdateResourcesAsync();
            await base.OnResume();

            await Task.Delay(300);
            for (int i = 0; i < swiperTabItems.ChildTabItems.Count; i++)
            {
                await JS.ScrollTo($"#{swiperTabItems.ChildTabItems[i].Id}", scrollTops[i], null, ScrollBehavior.Auto);
            }

            contentLoading = false;
            await InvokeAsync(StateHasChanged);
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

            var flag = await ResourceService.DeleteUnusedResourcesAsync(_ => true);
            if (flag)
            {
                if (swiperTabItems.ActiveItem is not null)
                {
                    await JS.ScrollTo($"#{swiperTabItems.ActiveItem.Id}", 0);
                }

                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }

            await UpdateResourcesAsync();
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl != args.PreviousUri)
            {
                await RecordScrollTops(args.NextUri);
                return;
            }

            if (thisPageUrl != args.NextUri) return;

            if (swiperTabItems.ActiveItem is null) return;

            await JS.ScrollTo($"#{swiperTabItems.ActiveItem.Id}", 0);
        }

        private async Task BeforePush(PushEventArgs args)
        {
            await RecordScrollTops(args.PreviousUri);
        }

        private async Task RecordScrollTops(string url)
        {
            if (url == thisPageUrl)
            {
                List<double> list = [];
                foreach (var swiperTabItem in swiperTabItems.ChildTabItems)
                {
                    double scrollTop = await JS.InvokeAsync<double>("elementScrollTop", $"#{swiperTabItem.Id}");
                    list.Add(scrollTop);
                }
                scrollTops = list;
            }

            contentLoading = true;
            _ = InvokeAsync(StateHasChanged);
        }
    }
}
