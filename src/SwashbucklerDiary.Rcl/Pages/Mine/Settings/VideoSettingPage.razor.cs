using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class VideoSettingPage
    {
        bool showConfimDelete;

        List<ResourceModel> videoResources = [];

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateVideoResourcesAsync();
                StateHasChanged();
            }
        }

        async Task UpdateVideoResourcesAsync()
        {
            videoResources = await ResourceService.QueryAsync(it => it.ResourceType == MediaResource.Video);
        }

        async Task DeleteUnusedResources()
        {
            showConfimDelete = false;
            var flag = await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceType == MediaResource.Video);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }

            await UpdateVideoResourcesAsync();
        }
    }
}
