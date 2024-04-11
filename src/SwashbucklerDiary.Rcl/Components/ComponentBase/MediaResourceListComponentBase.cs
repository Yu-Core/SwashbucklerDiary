using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaResourceListComponentBase : MyComponentBase
    {
        protected readonly int loadCount = 20;

        protected bool firstLoad = true;

        protected List<ResourceModel> previousValue = [];

        protected List<ResourceModel> LoadedItems = [];

        [CascadingParameter(Name = "ScrollElementId")]
        public string? ScrollElementId { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        protected bool ShowLoadMore => LoadedItems.Count != 0 && LoadedItems.Count < Value.Count;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (previousValue != Value)
            {
                previousValue = Value;
                int loadedCount = LoadedItems.Count;
                LoadedItems = [];
                LoadedItems = MockRequest(loadedCount);
            }
        }

        protected void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (firstLoad)
            {
                firstLoad = false;
                return;
            }

            var append = MockRequest();

            LoadedItems.AddRange(append);
        }

        protected virtual List<ResourceModel> MockRequest(int requestCount = 0)
        {
            if (requestCount < loadCount)
            {
                requestCount = loadCount;
            }

            return Value.Skip(LoadedItems.Count).Take(requestCount).ToList();
        }
    }
}
