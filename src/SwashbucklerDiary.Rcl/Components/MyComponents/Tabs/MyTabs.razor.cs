using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyTabs
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public StringNumber? Value { get; set; }

        [Parameter]
        public EventCallback<StringNumber?> ValueChanged { get; set; }

        [Parameter]
        public List<string> Items { get; set; } = [];
    }
}
