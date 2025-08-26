using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaResourceComponentBase : CardComponentBase<ResourceModel>
    {
        protected MediaResourcePath? mediaResourcePath;

        [Inject]
        protected IMediaResourceManager MediaResourceManager { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            mediaResourcePath = MediaResourceManager.ToMediaResourcePath(NavigationManager, Value.ResourceUri);
        }
    }
}
