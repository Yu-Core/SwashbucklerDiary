using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectIconDialog : DialogComponentBase
    {
        private List<IconListItem> iconListItems = [];

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

        [Parameter]
        public bool Mandatory { get; set; }

        protected override void OnInitialized()
        {
            SetIconListItems();

            base.OnInitialized();
        }

        private void SetIconListItems()
        {
            iconListItems = Items.Select(it => new IconListItem()
            {
                Name = it.Key,
                Icon = it.Value,
                Text = Text?.Invoke(it)
            }).ToList();
        }
    }
}
