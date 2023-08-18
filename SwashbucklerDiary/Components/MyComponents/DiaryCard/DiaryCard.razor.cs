using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool ShowMenu;
        private string? Title;
        private string? Text;
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
            SetTitleAndText();
            LoadView();
            base.OnInitialized();
        }

        private bool HasTitle => !string.IsNullOrWhiteSpace(Value?.Title);
        private bool HasContent => !string.IsNullOrWhiteSpace(Value?.Content);
        private DateTime Time => Value!.CreateTime;
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

        private void SetTitleAndText()
        {
            Title = GetTitle();
            Text = GetText();
        }

        private string? GetTitle()
        {
            if (HasTitle)
            {
                return Value!.Title!;
            }

            if (HasContent)
            {
                string[] separators = { ",", "，", ".", "。", "?", "？", "!", "！", ";", "；", "\n" }; // 定义分隔符
                string[] sentences = Value!.Content!.Split(separators, StringSplitOptions.RemoveEmptyEntries); // 拆分文本
                if (sentences.Length > 0)
                {
                    string firstSentence = sentences[0];
                    return firstSentence;
                }
            }

            return null;
        }

        private string? GetText()
        {
            var text = TextInterception(Value!.Content, 1000) ?? string.Empty;
            if (!HasTitle && Title is not null)
            {
                int length = Title.Length + 1;
                if (HasContent && text.Length > length)
                {
                    text = text?.Substring(length);
                }
            }

            return text;
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
