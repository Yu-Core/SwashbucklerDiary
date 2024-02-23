using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCard : CardComponentBase<DiaryModel>
    {
        private string? title;

        private string? text;

        private string? weatherIcon;

        private string? moodIcon;

        private DiaryModel previousValue = new();

        [Inject]
        private IIconService IconService { get; set; } = default!;

        [CascadingParameter]
        public DiaryCardList DiaryCardList { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            SetContent();
        }

        private bool IsActive => Value.Id == DiaryCardList.SelectedItemValue.Id;

        private bool ShowMenu => DiaryCardList.ShowMenu;

        private string ValueContent => Value.Content ?? string.Empty;

        private bool IsTop => Value.Top;

        private bool ShowIcon => DiaryCardList.ShowIcon;

        private string? DateFormat => DiaryCardList.DateFormat;

        private string? Date => DateFormat is null ? null : Value.CreateTime.ToString(DateFormat);

        private string? CreateDiaryTitle()
        {
            if (!string.IsNullOrWhiteSpace(Value.Title))
            {
                return Value.Title;
            }
            else
            {
                return CreateTitleFormText();
            }
        }

        private string? CreateDiaryText()
        {
            string text = SubText(Value.Content, 0, 2000);
            if (string.IsNullOrWhiteSpace(Value.Title))
            {
                int index = ExtractTitleIndex();
                var subText = SubText(text, index + 1);
                if (subText != string.Empty)
                {
                    return subText;
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

        private void ToRead()
        {
            NavigateService.PushAsync($"read?Id={Value.Id}");
        }

        private string CreateTitleFormText()
        {
            int index = ExtractTitleIndex();
            if (index > -1)
            {
                return ValueContent.Substring(0, index + 1);
            }
            else
            {
                return SubText(ValueContent, 0, 200);
            }
        }

        private int ExtractTitleIndex()
        {
            char[] separators = [']', '。', '\n']; // 定义分隔符
            int index = ValueContent.IndexOfAny(separators);
            if (index == -1 || index == ValueContent.Length - 1)
            {
                index = ValueContent.IndexOf(". ");
                if (index > -1)
                {
                    index++;
                }
            }

            return index != ValueContent.Length - 1 ? index : -1;
        }

        private void SetContent()
        {
            if (previousValue.Id != Value.Id)
            {
                previousValue = Value;
                title = CreateDiaryTitle();
                text = CreateDiaryText();
                weatherIcon = string.IsNullOrWhiteSpace(Value.Weather) ? null : IconService.GetWeatherIcon(Value.Weather);
                moodIcon = string.IsNullOrWhiteSpace(Value.Mood) ? null : IconService.GetMoodIcon(Value.Mood);
            }
        }
    }
}
