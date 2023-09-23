using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class SelectIconDialog : DialogComponentBase
    {
        private List<IconListItem> IconListItems = new();

        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public Dictionary<string, string> Items { get; set; } = new();
        [Parameter]
        public StringNumber? Item { get; set; }
        [Parameter]
        public EventCallback<StringNumber> ItemChanged { get; set; }
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
                IconListItems.Add(iconListItem);
            }

            base.OnInitialized();
        }
    }
}
