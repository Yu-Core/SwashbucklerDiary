using DocumentFormat.OpenXml.Spreadsheet;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class CardListComponentBase<T> : ImportantComponentBase where T : BaseModel, new()
    {
        protected List<T> _value = [];

        protected string? sortItem;

        protected bool showSort;

        protected bool showStatisticsCard;

        protected Dictionary<string, Func<IEnumerable<T>, IEnumerable<T>>> sortOptions = [];

        protected List<DynamicListItem> menuItems = [];

        protected MultiMenu? multiMenu;

        protected Dictionary<string, object> previousActivatorAttributes = [];

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public virtual List<T> Value
        {
            get => Sort(_value).ToList();
            set => _value = value;
        }

        public void Sort()
        {
            showSort = true;
            InvokeAsync(StateHasChanged);
        }

        public bool ShowMenu { get; set; }

        public T SelectedItemValue { get; set; } = new();

        protected List<string> SortItems => sortOptions.Keys.ToList();

        protected Dictionary<string, object>? ActivatorAttributes => multiMenu?.ActivatorAttributes;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            showStatisticsCard = SettingService.Get<bool>(Setting.StatisticsCard);
        }

        protected virtual IEnumerable<T> Sort(IEnumerable<T> value)
        {
            if (sortItem is null || !sortOptions.TryGetValue(sortItem, out Func<IEnumerable<T>, IEnumerable<T>>? sortOption))
            {
                return value;
            }

            return sortOption.Invoke(value);
        }

        protected void OpenMenu((T value, Dictionary<string, object> activatorAttributes) args)
        {
            if (SelectedItemValue.Id != args.value.Id)
            {
                SelectedItemValue = args.value;
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

            ShowMenu = true;
            InvokeAsync(StateHasChanged);
        }

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
