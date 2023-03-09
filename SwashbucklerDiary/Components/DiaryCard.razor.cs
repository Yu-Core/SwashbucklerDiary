using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<ViewListItem> ViewListItems = new();

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

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private DateTime Time => Value!.CreateTime;
        private string? Title
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Value!.Title))
                {
                    return Value!.Title;
                }
                foreach (var item in Text!.Split("\n"))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        return item;
                    }
                }
                return I18n.T("Diary.Untitled");
            }
        }
        private string? Text => Value!.Content;
        private bool IsTop => Value!.Top;
        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";

        private void LoadView()
        {
            ViewListItems = new()
            {
                new("Diary.Tag","mdi-label-outline",()=>OnTag.InvokeAsync()),
                new("Share.Copy","mdi-content-copy",()=>OnCopy.InvokeAsync()),
                new("Share.Delete","mdi-delete-outline",()=>OnDelete.InvokeAsync()),
                new(TopText(),"mdi-format-vertical-align-top",()=>OnTopping.InvokeAsync()),
                new("Diary.Export","mdi-export",()=>ToDo())
            };
        }
    }
}
