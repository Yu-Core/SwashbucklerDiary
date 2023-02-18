using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
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

        private DateTime Time => Value!.CreateTime;
        private string? Title
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(Value!.Title))
                {
                    return Value!.Title;
                }
                foreach (var item in Text!.Split("\n"))
                {
                    if(!string.IsNullOrWhiteSpace(item))
                    {
                        return item;
                    }
                }
                return I18n.T("Diary.Untitled");
            }
        }
        private string? Text => Value!.Content;
        private bool IsTop => Value!.Top;
    }
}
