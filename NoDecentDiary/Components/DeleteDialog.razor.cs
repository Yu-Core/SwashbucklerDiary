using Microsoft.AspNetCore.Components;

namespace NoDecentDiary.Components
{
    public partial class DeleteDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Content { get; set; }
        [Parameter]
        public EventCallback OnOK { get; set; }
    }
}
