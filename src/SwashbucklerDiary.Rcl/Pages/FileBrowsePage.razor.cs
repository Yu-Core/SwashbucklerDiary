using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
            new("Image","image"),
            new("Audio","audio"),
            new("Video","video"),
        ];

        private bool showDelete;

        private bool showMenu;

        private bool showExport;

        private StringNumber tab = 0;

        private List<ResourceModel> imageResources = [];

        private List<ResourceModel> audioResources = [];

        private List<ResourceModel> videoResources = [];

        private List<DynamicListItem> menuItems = [];

        [Inject]
        protected IResourceService ResourceService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<FileBrowsePage> Logger { get; set; } = default!;

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
                new(this, "Delete unused files","mdi:mdi-delete-outline", ()=>showDelete=true),
                new(this, "Export files","mdi:mdi-export", ()=>showExport=true),
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
                await AlertService.SuccessAsync(I18n.T("Delete successfully"));
            }
        }

        private async Task Export(List<MediaResource> resourceKinds)
        {
            showExport = false;

            var permission = await PlatformIntegration.TryStorageWritePermission();
            if (!permission)
            {
                await AlertService.InfoAsync(I18n.T("Please grant permission for storage writing"));
                return;
            }

            AlertService.StartLoading();
            try
            {
                var resources = await ResourceService.QueryAsync(it => resourceKinds.Contains(it.ResourceType));
                if (resources.Count == 0)
                {
                    await AlertService.InfoAsync(I18n.T("No file"));
                    return;
                }

                var path = await DiaryFileManager.ExportResourceFileAsync(resources);
                if (!string.IsNullOrEmpty(path))
                {
                    bool flag = await PlatformIntegration.SaveFileAsync(path);
                    if (flag)
                    {
                        await AlertService.SuccessAsync(I18n.T("Export successfully"));
                        await HandleAchievements(Achievement.Export);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Create file wrong");
                await AlertService.ErrorAsync(I18n.T("Export failed"));
            }
            finally
            {
                AlertService.StopLoading();
            }
        }
    }
}
