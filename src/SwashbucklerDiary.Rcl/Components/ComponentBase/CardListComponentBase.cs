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

        protected MultiMenu? multiMenu;

        protected Dictionary<string, object> previousActivatorAttributes = [];

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

        protected virtual T SelectedItem { get; set; } = new();

        protected List<string> SortItems => sortOptions.Keys.ToList();

        protected Dictionary<string, object>? ActivatorAttributes => multiMenu?.ActivatorAttributes;

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

            showStatisticsCard = SettingService.Get<bool>(Setting.StatisticsCard);
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

        protected void OpenMenu((T value, Dictionary<string, object> activatorAttributes) args)
        {
            if (SelectedItem.Id != args.value.Id)
            {
                SelectedItem = args.value;
                if (ActivatorAttributes is not null)
                {
                    //清除旧的Activator的属性，必须这样写，直接Clear是无效的
                    foreach (var key in ActivatorAttributes.Keys)
                    {
                        previousActivatorAttributes[key] = false;
                    }

                    foreach (var item in ActivatorAttributes)
                    {
                        args.activatorAttributes[item.Key] = item.Value;
                    }

                    previousActivatorAttributes = args.activatorAttributes;
                }
            }

            showMenu = true;
            InvokeAsync(StateHasChanged);
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
