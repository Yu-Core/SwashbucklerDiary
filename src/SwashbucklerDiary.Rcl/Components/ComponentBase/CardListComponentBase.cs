using DocumentFormat.OpenXml.Spreadsheet;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class CardListComponentBase<T> : ImportantComponentBase where T : BaseModel, new()
    {
        protected List<T> InternalValue = [];

        protected bool showSort;

        protected bool showStatisticsCard;

        protected bool showMenu;

        protected Dictionary<string, Func<IEnumerable<T>, IEnumerable<T>>> sortOptions = [];

        protected List<DynamicListItem> menuItems = [];

        protected Dictionary<string, object> menuActivatorAttributes = [];

        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Parameter]
        public List<T> Value
        {
            get => GetValue<List<T>>() ?? [];
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<List<T>> ValueChanged { get; set; }

        public void OpenSortDialog()
        {
            showSort = true;
            InvokeAsync(StateHasChanged);
        }

        [Parameter]
        public bool HideStatisticsCard { get; set; }

        protected virtual T SelectedItem { get; set; } = new();

        protected List<string> SortItems => sortOptions.Keys.ToList();

        protected bool ShowStatisticsCard => Value.Count > 0 && showStatisticsCard && !HideStatisticsCard;

        protected string? SortItem
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showStatisticsCard = SettingService.Get(s => s.StatisticsCard);
        }

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<List<T>>(nameof(Value), UpdateInternalValue, immediate: true)
                   .Watch<string?>(nameof(SortItem), UpdateInternalValue, immediate: true);
        }

        protected virtual void UpdateInternalValue()
        {
            InternalValue = Sort(Value).ToList();
            StateHasChanged();
        }

        protected virtual IEnumerable<T> Sort(IEnumerable<T> value)
        {
            if (SortItem is null || !sortOptions.TryGetValue(SortItem, out Func<IEnumerable<T>, IEnumerable<T>>? sortOption))
            {
                return value;
            }

            return sortOption.Invoke(value);
        }

        protected async Task OpenMenu((T value, Dictionary<string, object> activatorAttributes) args)
        {
            showMenu = false;
            await Task.Delay(16);

            SelectedItem = args.value;
            menuActivatorAttributes = args.activatorAttributes;
            showMenu = true;
        }

        protected bool RemoveSelectedItem()
        {
            var index = Value.FindIndex(it => it.Id == SelectedItem.Id);
            if (index < 0)
            {
                return false;
            }

            Value.RemoveAt(index);
            NotifyValueChanged();
            return true;
        }

        protected void NotifyValueChanged()
        {
            Value = [.. Value];
            if (ValueChanged.HasDelegate)
            {
                ValueChanged.InvokeAsync(Value);
            }
        }

        private void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged || !e.SmChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}
