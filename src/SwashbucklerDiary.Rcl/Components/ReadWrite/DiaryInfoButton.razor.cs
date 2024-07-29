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

        string InternalClass => $"{Class} text--secondary{(ReadOnly ? " m-btn--readonly" : "")}";
    }
}
