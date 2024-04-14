using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaResourceListComponentBase : MyComponentBase
    {
        protected readonly int loadCount = 20;

        protected int previousLoadedCount;

        protected MInfiniteScroll mInfiniteScroll = default!;

        protected List<ResourceModel> previousValue = [];

        protected List<ResourceModel> LoadedItems = [];

        [CascadingParameter(Name = "ScrollElementId")]
        public string? ScrollElementId { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        protected bool HasMore => LoadedItems.Count < Value.Count;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                previousLoadedCount = LoadedItems.Count;
                LoadedItems = [];
                if (mInfiniteScroll is not null)
                {
                    await mInfiniteScroll.ResetAsync();
                }
            }
        }

        protected void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            var append = MockRequest();

            LoadedItems.AddRange(append);

            args.Status = HasMore ? InfiniteScrollLoadStatus.Ok : InfiniteScrollLoadStatus.Empty;
        }

        protected virtual List<ResourceModel> MockRequest()
        {
            int requestCount = Math.Max(previousLoadedCount, loadCount);

            if (previousLoadedCount != 0)
            {
                previousLoadedCount = 0;
            }

            return Value.Skip(LoadedItems.Count).Take(requestCount).ToList();
        }
    }
}
