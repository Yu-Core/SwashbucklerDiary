using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<DynamicListItem> ListItemModels = new();

        [Inject]
        private IconService IconService { get; set; } = default!;

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
        [Parameter]
        public bool Icon { get; set; }
        [Parameter]
        public string? DateFormat { get; set; }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private DateTime Time => Value!.CreateTime;
        private string? Title => GetTitle();
        private string? Text => TextInterception(Value!.Content, 1000);
        private bool IsTop => Value!.Top;
        private bool IsPrivate => Value!.Private;
        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";
        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";
        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this,"Diary.Tag","mdi-label-outline",()=>OnTag.InvokeAsync(Value)),
                new(this, "Share.Copy","mdi-content-copy",() => OnCopy.InvokeAsync(Value)),
                new(this, "Share.Delete","mdi-delete-outline",() => OnDelete.InvokeAsync(Value)),
                new(this, TopText,"mdi-format-vertical-align-top",() => OnTopping.InvokeAsync(Value)),
                new(this, "Diary.Export","mdi-export",() => OnExport.InvokeAsync(Value))
            };

            if (Privacy)
            {
                ListItemModels.Add(new(this, PrivateText, PrivateIcon, () => OnPrivacy.InvokeAsync(Value)));
            }
        }

        private string? GetWeatherIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            return IconService.GetWeatherIcon(key);
        }

        private string? GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            return IconService.GetMoodIcon(key);
        }

        private string GetTitle()
        {
            if (!string.IsNullOrWhiteSpace(Value!.Title))
            {
                return Value!.Title;
            }

            if (!string.IsNullOrWhiteSpace(Text))
            {
                string[] separators = { ",", "，", ".", "。", "?", "？", "!", "！", ";", "；", "\n" }; // 定义分隔符
                string[] sentences = Text.Split(separators, StringSplitOptions.RemoveEmptyEntries); // 拆分文本
                if(sentences.Length > 0)
                {
                    string firstSentence = sentences[0];
                    return firstSentence;
                }
            }

            return I18n.T("Diary.Untitled");
        }

        private static string? TextInterception(string? text, int endIndex)
        {
            int len = text is null ? 0 : text.Length;
            if (len > endIndex)
            {
                return text!.Substring(0, endIndex);
            }

            return text;
        }
    }
}
