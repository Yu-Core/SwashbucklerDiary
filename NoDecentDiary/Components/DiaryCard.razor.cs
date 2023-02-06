using Microsoft.AspNetCore.Components;
using NoDecentDiary.Models;

namespace NoDecentDiary.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool ShowMenu;

        [Parameter]
        public DiaryModel? Value { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public EventCallback OnTopping { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnCopy { get; set; }
        [Parameter]
        public EventCallback OnTag { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }

        private DateTime Date => Value!.CreateTime;
        private string? Title => Value!.Title;
        private string? Text => Value!.Content;
        private bool IsTop => Value!.Top;
    }
}
