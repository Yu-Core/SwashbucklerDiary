using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class ImageSettingPage : ImportantComponentBase
    {
        bool ShowConfimDelete;

        List<ResourceModel> ImageResources = new();

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateImageResourcesAsync();
            await base.OnInitializedAsync();
        }

        async Task UpdateImageResourcesAsync()
        {
            ImageResources = await ResourceService.QueryAsync(it => it.ResourceType == ResourceType.Image);
        }

        async Task DeleteUnusedImageResources()
        {
            ShowConfimDelete = false;
            var flag = await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceType == ResourceType.Image);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }

            var resources = await ResourceService.QueryAsync(it => it.ResourceType == ResourceType.Image);
            ImageResources = resources;

            var resourceUris = resources.Select(it => it.ResourceUri!).ToList();
            AppDataService.DeleteAppDataFileByCustomPath(resourceUris, ResourceType.Image);
            
        }

    }
}
