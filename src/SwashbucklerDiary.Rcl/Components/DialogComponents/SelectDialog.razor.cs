using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public Dictionary<string, string> Items { get; set; } = [];

        [Parameter]
        public StringNumber? Value { get; set; }

        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }

        [Parameter]
        public bool Mandatory { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, string>, string>? ItemText { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, string>, string>? ItemValue { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, string>, string>? ItemIcon { get; set; }

        private string InternalItemText(KeyValuePair<string, string> item)
        {
            if (ItemText is not null)
            {
                return ItemText.Invoke(item);
            }

            return I18n.T(item.Key);
        }

        private string InternalItemValue(KeyValuePair<string, string> item)
        {
            if (ItemValue is not null)
            {
                return ItemValue.Invoke(item);
            }

            return item.Value;
        }

        private string InternalItemIcon(KeyValuePair<string, string> item)
        {
            if (ItemIcon is not null)
            {
                return ItemIcon.Invoke(item);
            }

            return string.Empty;
        }
    }
}
