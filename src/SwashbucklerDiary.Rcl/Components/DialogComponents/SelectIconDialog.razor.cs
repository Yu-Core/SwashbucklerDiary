using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectIconDialog : DialogComponentBase
    {
        private readonly List<IconListItem> iconListItems = [];

        [Parameter]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public Dictionary<string, string> Items { get; set; } = [];

        [Parameter]
        public StringNumber? Value { get; set; }

        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }

        [Parameter]
        public Func<KeyValuePair<string, string>, string>? Text { get; set; }

        protected override void OnInitialized()
        {
            foreach (var item in Items)
            {
                var iconListItem = new IconListItem()
                {
                    Name = item.Key,
                    Icon = item.Value,
                    Text = Text?.Invoke(item)
                };
                iconListItems.Add(iconListItem);
            }

            base.OnInitialized();
        }
    }
}
