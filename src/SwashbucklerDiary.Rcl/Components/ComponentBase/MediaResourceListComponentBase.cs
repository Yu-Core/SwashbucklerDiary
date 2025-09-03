using Masa.Blazor;
using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
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

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElementId")]
        public string? ScrollElementId { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        protected string InfiniteScrollClass => new CssBuilder()
            .Add("py-0", !HasMore)
            .ToString();

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
                new(this, "Save", "mdi:mdi-tray-arrow-down", Save),
                new(this, "Delete", "mdi:mdi-delete-outline", Delete)
            ];
        }

        private async Task Delete()
        {
            List<Guid> diaryIds = await ResourceService.GetDiaryIdsAsync(SelectedItem.ResourceUri);
            if (diaryIds.Count > 0)
            {
                await AlertService.InfoAsync(I18n.T("This file is using"));
            }
            else
            {
                await ResourceService.DeleteUnusedResourcesAsync(it => it.ResourceUri == SelectedItem.ResourceUri);
                loadedItems.Remove(SelectedItem);
                Value.Remove(SelectedItem);
                await AlertService.SuccessAsync(I18n.T("Delete successfully"));
            }
        }

        private async Task Save()
        {
            if (SelectedItem.ResourceUri is null)
            {
                return;
            }

            string? filePath = null;

            AlertService.StartLoading();
            try
            {
                var mediaResourcePath = MediaResourceManager.ToMediaResourcePath(NavigationManager, SelectedItem.ResourceUri);
                filePath = await MediaResourceManager.ToFilePathAsync(mediaResourcePath);
            }
            finally
            {
                AlertService.StopLoading();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                await AlertService.ErrorAsync(I18n.T("File does not exist"));
                return;
            }

            await PlatformIntegration.SaveFileAsync(filePath);
        }

        private async Task ViewReferenced()
        {
            if (SelectedItem.ResourceUri is null)
            {
                return;
            }

            List<Guid> diaryIds = await ResourceService.GetDiaryIdsAsync(SelectedItem.ResourceUri);

            int count = diaryIds.Count;
            if (count < 1)
            {
                await AlertService.InfoAsync(I18n.T("This file is not used"));
                return;
            }
            else if (count == 1)
            {
                To($"read/{diaryIds[0]}");
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
