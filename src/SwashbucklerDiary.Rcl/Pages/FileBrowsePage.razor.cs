using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class FileBrowsePage : ImportantComponentBase
    {
        private readonly List<TabListItem> tabListItems =
        [
            new("FileBrowse.Image.Name","image"),
            new("FileBrowse.Audio.Name","audio"),
            new("FileBrowse.Video.Name","video"),
        ];

        private bool showDelete;

        private bool showMenu;

        private StringNumber tab = 0;

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

        protected override async Task OnResume()
        {
            await UpdateResourcesAsync();
            await base.OnResume();
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
                await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
            }
        }
    }
}
