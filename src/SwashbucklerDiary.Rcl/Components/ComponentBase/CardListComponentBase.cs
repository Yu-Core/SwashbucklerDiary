using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class CardListComponentBase<T> : ImportantComponentBase where T : BaseModel, new()
    {
        protected List<T> _value = [];

        protected string? sortItem;

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

        protected virtual T SelectedItemValue { get; set; } = new();

        protected List<string> SortItems => sortOptions.Keys.ToList();

        protected Dictionary<string, object>? ActivatorAttributes => multiMenu?.ActivatorAttributes;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

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

            showMenu = true;
            InvokeAsync(StateHasChanged);
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
