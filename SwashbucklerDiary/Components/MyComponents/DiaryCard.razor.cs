using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

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
        public EventCallback<DiaryModel> OnTopping { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnDelete { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnCopy { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnTag { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnExport { get; set; }
        [Parameter]
        public bool Privacy { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnPrivacy { get; set; }
        [Parameter]
        public EventCallback<DiaryModel> OnClick { get; set; }

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
        private bool IsPrivate => Value!.Private;
        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";
        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";
        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

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

            if (Privacy)
            {
                ListItemModels.Add(new(PrivateText, PrivateIcon, () => OnPrivacy.InvokeAsync(Value)));
            }
        }
    }
}
