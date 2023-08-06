using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class ImagePage : PageComponentBase
    {
        bool ShowConfimDelete;
        List<ResourceModel> ImageResources = new();

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

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
            bool flag = await ResourceService.DeleteUnusedResourcesAsync(it=>it.ResourceType == ResourceType.Image);
            if(flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Success(I18n.T("Share.DeleteFail"));

            }
        }

    }
}
