using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ImageSettingPage : ImportantComponentBase
    {
        bool showConfimDelete;

        List<ResourceModel> imageResources = [];

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateImageResourcesAsync();
            await base.OnInitializedAsync();
        }

        async Task UpdateImageResourcesAsync()
        {
            imageResources = await ResourceService.QueryAsync(it => it.ResourceType == MediaResource.Image);
        }

        async Task DeleteUnusedImageResources()
        {
            showConfimDelete = false;
            var flag = await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceType == MediaResource.Image);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
        }

    }
}
