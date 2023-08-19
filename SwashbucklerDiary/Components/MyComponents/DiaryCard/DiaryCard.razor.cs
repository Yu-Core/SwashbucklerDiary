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
                char[] separators = { ',', '，', '.', '。', '?', '？', '!', '！', ';', '；', '\n' }; // 定义分隔符
                int index = Value!.Content!.IndexOfAny(separators);
                if (index > -1)
                {
                    return Value!.Content!.Substring(0, index + 1);
                }
            }

            return null;
        }

        private string? GetText()
        {
            string text = SubText(Value!.Content, 0, 1000);
            if (!HasTitle && Title is not null && HasContent)
            {
                var subText = SubText(text, Title.Length);
                if (!string.IsNullOrWhiteSpace(subText))
                {
                    text = subText;
                }
            }

            return text;
        }

        private static string SubText(string? text, int startIndex, int? length = null)
        {
            if (text == null)
            {
                return string.Empty;
            }

            int textLength = text.Length;

            if (startIndex < 0 || startIndex >= textLength)
            {
                return string.Empty;
            }

            if (length == null)
            {
                length = textLength - startIndex;
            }
            else if (length < 0)
            {
                return string.Empty;
            }

            int endIndex = startIndex + length.Value;

            if (endIndex > textLength)
            {
                endIndex = textLength;
            }

            return text.Substring(startIndex, endIndex - startIndex);
        }
    }
}
