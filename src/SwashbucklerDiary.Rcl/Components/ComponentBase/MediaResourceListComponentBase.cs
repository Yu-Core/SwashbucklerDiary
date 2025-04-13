using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaResourceListComponentBase : MyComponentBase
    {
        protected bool showMenu;

        protected readonly int loadCount = 20;

        protected int previousLoadedCount;

        protected MInfiniteScroll mInfiniteScroll = default!;

        protected List<ResourceModel> previousValue = [];

        protected List<ResourceModel> loadedItems = [];

        protected List<DynamicListItem> menuItems = [];

        protected Dictionary<string, object> menuActivatorAttributes = [];

        [Inject]
        protected IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        protected IResourceService ResourceService { get; set; } = default!;

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElementId")]
        public string? ScrollElementId { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        protected virtual ResourceModel SelectedItem { get; set; } = new();

        protected bool HasMore => loadedItems.Count < Value.Count;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                previousLoadedCount = loadedItems.Count;
                loadedItems = [];
                if (mInfiniteScroll is not null)
                {
                    await mInfiniteScroll.ResetAsync();
                }
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            var append = MockRequest();

            loadedItems.AddRange(append);

            args.Status = HasMore ? InfiniteScrollLoadStatus.Ok : InfiniteScrollLoadStatus.Empty;
        }

        protected virtual List<ResourceModel> MockRequest()
        {
            int requestCount = Math.Max(previousLoadedCount, loadCount);

            if (previousLoadedCount != 0)
            {
                previousLoadedCount = 0;
            }

            return Value.Skip(loadedItems.Count).Take(requestCount).ToList();
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "View Used", "file_export", ViewReferenced),
                new(this, "Save", "mdi:mdi-tray-arrow-down", Save)
            ];
        }

        private async Task Save()
        {
            if (SelectedItem.ResourceUri is null)
            {
                return;
            }

            await MediaResourceManager.SaveFileAsync(SelectedItem.ResourceUri);
        }

        private async Task ViewReferenced()
        {
            if (SelectedItem.ResourceUri is null)
            {
                return;
            }

            var resource = await ResourceService.FindIncludesAsync(SelectedItem.ResourceUri);
            int count = resource?.Diaries?.Count ?? 0;
            if (count < 1)
            {
                await PopupServiceHelper.Info(I18n.T("This file is not used"));
                return;
            }
            else if (count == 1)
            {
                var diary = resource?.Diaries?.FirstOrDefault();
                if (diary is not null)
                {
                    To($"read/{diary.Id}");
                }
            }
            else if (count > 1)
            {
                To($"resourceDetails?id={SelectedItem.ResourceUri}");
            }
        }

        protected async Task OpenMenu((ResourceModel value, Dictionary<string, object> activatorAttributes) args)
        {
            showMenu = false;
            await Task.Delay(16);

            SelectedItem = args.value;
            menuActivatorAttributes = args.activatorAttributes;
            showMenu = true;
        }
    }
}
