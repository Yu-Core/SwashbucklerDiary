using BlazorComponent;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class SelectIconDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public Dictionary<string, string> Items { get; set; } = new();
        [Parameter] 
        public StringNumber? Item { get; set; }
        [Parameter]
        public EventCallback<StringNumber> ItemChanged { get; set; }
        [Parameter]
        public Func<KeyValuePair<string,string>,string>? Text { get; set; }
    }
}
