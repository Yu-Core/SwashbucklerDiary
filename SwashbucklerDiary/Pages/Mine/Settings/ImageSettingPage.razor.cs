using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class ImageSettingPage : PageComponentBase
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

        async Task DeleteUnusedImageResource()
        {
            ShowConfimDelete = false;
            var resources = await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceType == ResourceType.Image);
            if (resources is not null && resources.Any())
            {
                ImageResources.RemoveAll(item => resources.Any(x => x.ResourceUri == item.ResourceUri));
                foreach (var resource in resources)
                {
                    await AppDataService.DeleteAppDataFileByCustomSchemeAsync(resource.ResourceUri!);
                }
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
        }

    }
}
