using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryInfoButton
    {
        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }

        bool ReadOnly => !OnClick.HasDelegate;

        string InternalClass => new CssBuilder()
            .Add(Class)
            .Add("diary-info-btn")
            .Add("text--secondary")
            .Add("m-btn--readonly", ReadOnly)
            .ToString();
    }
}
