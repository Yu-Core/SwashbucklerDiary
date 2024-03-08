using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class RadioDialog<TItem, TItemValue> : DialogComponentBase
    {
        [Parameter]
        public TItemValue Value { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public EventCallback<TItemValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TItemValue> OnChange { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public ICollection<TItem> Items { get; set; } = default!;

        [Parameter]
        public Func<TItem, string>? ItemText { get; set; }

        [Parameter]
        public Func<TItem, TItemValue>? ItemValue { get; set; }

        [Parameter]
        public bool Row { get; set; }

        private string MRadioColor => Dark ? "white" : "black";

        private async Task InternalValueChanged(TItemValue value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }

        private string InternalItemText(TItem item)
        {
            //指定选项显示文本
            if (ItemText is not null)
            {
                return ItemText.Invoke(item);
            }
            //字典，选项显示文本默认为Key
            if (item is KeyValuePair<string, TItemValue> k)
            {
                return I18n.T(k.Key);
            }
            //字符串集合，选项显示文本默认为字符串
            if (item is string s)
            {
                return I18n.T(s);
            }

            return string.Empty;
        }

        private TItemValue InternalItemValue(TItem item)
        {
            //指定选项值
            if (ItemValue is not null)
            {
                return ItemValue.Invoke(item);
            }

            //字典，选项值默认为Value
            if (item is KeyValuePair<string, TItemValue> k)
            {
                return k.Value;
            }

            //普通集合，选项值默认为Value
            if (item is TItemValue itemValue)
            {
                return itemValue;
            }

            return default!;
        }
    }
}
