using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ResourceDetailsPage : DiariesPageComponentBase
    {
        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string scrollContainerSelector = string.Empty;

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? Id { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
        }

        protected override async Task UpdateDiariesAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                await NavigateToBack();
                return;
            }

            var resource = await ResourceService.FindIncludesAsync(Id);
            if (resource is null)
            {
                await NavigateToBack();
                return;
            }

            Diaries = resource?.Diaries ?? [];
        }
    }
}