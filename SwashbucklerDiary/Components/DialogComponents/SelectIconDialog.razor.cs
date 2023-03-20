using BlazorComponent;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class SelectIconDialog : DialogComponentBase
    {
        private List<string> ItemsKeys = new();

        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public Dictionary<string, string> Items { get; set; } = new();
        [Parameter] 
        public string? Item { get; set; }
        [Parameter]
        public EventCallback<string> ItemChanged { get; set; }
        [Parameter]
        public Func<KeyValuePair<string,string>,string>? Text { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ItemsKeys = Items.Keys.ToList();
        }

        private StringNumber GetSelectedIndex()
        {
            if (string.IsNullOrEmpty(Item))
            {
                return -1;
            }
            return ItemsKeys.IndexOf(Item);
        }

        private void SelectedIndexChanged(StringNumber value)
        {
            string selectedItem = string.Empty;
            if (value != null)
            {
                selectedItem = Items.ElementAt(value.ToInt32()).Key;
            }

            Item = selectedItem;
            if (ItemChanged.HasDelegate)
            {
                ItemChanged.InvokeAsync(selectedItem);
            }
        }
    }
}
