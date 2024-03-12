using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AudioSettingPage : ImportantComponentBase
    {
        bool showConfimDelete;

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        async Task DeleteUnusedResources()
        {
            showConfimDelete = false;
            var flag = await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceType == MediaResource.Audio);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
        }
    }
}
