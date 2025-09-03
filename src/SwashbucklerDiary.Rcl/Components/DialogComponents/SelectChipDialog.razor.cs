using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectChipDialog<TValue> : DialogComponentBase
    {
        private StringNumber? internalValue;
        private List<StringNumber> internalValues = [];

        [Parameter]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public Dictionary<string, TValue> Items { get; set; } = [];

        [Parameter]
        public TValue Value
        {
            get => GetValue<TValue>();
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public List<TValue> Values
        {
            get => GetValue<List<TValue>>([])!;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<List<TValue>> ValuesChanged { get; set; }

        [Parameter]
        public bool Multiple { get; set; }

        [Parameter]
        public bool Mandatory { get; set; }

        [Parameter]
        public bool? ShowText { get; set; }

        [Parameter]
        public EventCallback<bool> ShowTextChanged { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, TValue>, string>? ItemText { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, TValue>, TValue>? ItemValue { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, TValue>, string>? ItemIcon { get; set; }

        private bool InternalShowText => ShowText ?? true;

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<TValue>(nameof(Value), UpdateInternalValue, immediate: true);
            watcher.Watch<List<TValue>>(nameof(Values), UpdateInternalValues, immediate: true);
        }

        private void UpdateInternalValue()
        {
            internalValue = Items.FirstOrDefault(it => EqualityComparer<TValue>.Default.Equals(Value, InternalItemValue(it))).Key;
        }

        private void UpdateInternalValues()
        {
            HashSet<TValue> valuesSet = new HashSet<TValue>(Values);
            internalValues = Items.Where(it => valuesSet.Contains(InternalItemValue(it))).Select(it => (StringNumber)it.Key).ToList();
        }

        private async Task InternalValueChanged(StringNumber? value)
        {
            if (internalValue == value)
            {
                return;
            }

            if (ValueChanged.HasDelegate)
            {
                var item = Items.FirstOrDefault(it => it.Key == value);
                await ValueChanged.InvokeAsync(InternalItemValue(item));
            }
        }

        private async Task InternalValuesChanged(List<StringNumber> values)
        {
            if (ValuesChanged.HasDelegate)
            {
                List<TValue> result = new List<TValue>();

                foreach (var key in values)
                {
                    var item = Items.FirstOrDefault(it => it.Key == key);
                    result.Add(InternalItemValue(item));
                }

                await ValuesChanged.InvokeAsync(result);
            }
        }

        private string? InternalItemText(KeyValuePair<string, TValue> item)
        {
            if (!InternalShowText)
            {
                return null;
            }

            if (ItemText is not null)
            {
                return ItemText.Invoke(item);
            }

            return I18n.T(item.Key);
        }

        private TValue InternalItemValue(KeyValuePair<string, TValue> item)
        {
            if (ItemValue is not null)
            {
                return ItemValue.Invoke(item);
            }

            return item.Value;
        }

        private string InternalItemIcon(KeyValuePair<string, TValue> item)
        {
            if (ItemIcon is not null)
            {
                return ItemIcon.Invoke(item);
            }

            return string.Empty;
        }

        private async Task UpdateShowText()
        {
            if (ShowText is null)
            {
                return;
            }

            ShowText = !ShowText;
            if (ShowTextChanged.HasDelegate)
            {
                await ShowTextChanged.InvokeAsync((bool)ShowText);
            }

            await SettingService.SetAsync(it => it.DiaryIconText, (bool)ShowText);
        }

        private async Task CloseOnContentClick()
        {
            if (Multiple)
            {
                return;
            }

            await InternalVisibleChanged(false);
        }
    }
}
