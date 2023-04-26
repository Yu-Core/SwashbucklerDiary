using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;
using Util.Reflection.Expressions.IntelligentGeneration.Extensions;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<ListItemModel> ListItemModels = new();

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
        public EventCallback OnExport { get; set; }
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

                if (!string.IsNullOrWhiteSpace(Text))
                {
                    foreach (var item in Text!.Split("\n"))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            return item;
                        }
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
            ListItemModels = new()
            {
                new("Diary.Tag","mdi-label-outline",() => OnTag.InvokeAsync(Value)),
                new("Share.Copy","mdi-content-copy",() => OnCopy.InvokeAsync(Value)),
                new("Share.Delete","mdi-delete-outline",() => OnDelete.InvokeAsync(Value)),
                new(TopText,"mdi-format-vertical-align-top",() => OnTopping.InvokeAsync(Value)),
                new("Diary.Export","mdi-export",() => OnExport.InvokeAsync(Value))
            };
        }
    }
}
