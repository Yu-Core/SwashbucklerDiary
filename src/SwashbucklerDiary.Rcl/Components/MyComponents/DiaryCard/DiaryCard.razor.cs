using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private bool showMenu;

        private List<DynamicListItem> menuItems = [];

        [Inject]
        private IIconService IconService { get; set; } = default!;

        [CascadingParameter]
        public DiaryCardList DiaryCardList { get; set; } = default!;

        [Parameter]
        public DiaryModel Value { get; set; } = default!;

        [Parameter]
        public string? Class { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        private string? Title => GetTitle();

        private string? Text => GetText();

        private string ValueContent => Value.Content ?? string.Empty;

        private DateTime Time => Value.CreateTime;

        private bool IsTop => Value.Top;

        private bool IsPrivate => Value.Private;

        private bool ShowPrivacy => DiaryCardList.ShowPrivacy;

        private bool ShowIcon => DiaryCardList.ShowIcon;

        private string? DateFormat => DiaryCardList.DateFormat;

        private string TopText()
            => IsTop ? "Diary.CancelTop" : "Diary.Top";

        private string PrivateText()
            => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";

        private string PrivateIcon()
            => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private string? WeatherIcon =>
            string.IsNullOrWhiteSpace(Value.Weather) ? null : IconService.GetWeatherIcon(Value.Weather);

        private string? MoodIcon =>
            string.IsNullOrWhiteSpace(Value.Mood) ? null : IconService.GetMoodIcon(Value.Mood);

        private void LoadView()
        {
            menuItems = new()
            {
                new(this,"Diary.Tag","mdi-label-outline",ChangeTag),
                new(this, "Share.Copy","mdi-content-copy",Copy),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
                new(this, TopText,"mdi-format-vertical-align-top",Topping),
                new(this, "Diary.Export","mdi-export",Export),
                new(this, "Share.Sort","mdi-sort-variant",Sort),
                new(this, PrivateText, PrivateIcon, MovePrivacy,()=>ShowPrivacy)
            };
        }

        private Task Topping()
        {
            return DiaryCardList.Topping(Value);
        }

        private void Delete()
        {
            DiaryCardList.Delete(Value);
        }

        private Task Copy()
        {
            return DiaryCardList.Copy(Value);
        }

        private Task ChangeTag()
        {
            return DiaryCardList.ChangeTag(Value);
        }

        private Task Export()
        {
            return DiaryCardList.Export(Value);
        }

        private void Sort()
            => DiaryCardList.Sort();

        private Task MovePrivacy()
        {
            return DiaryCardList.MovePrivacy(Value);
        }

        private string? GetTitle()
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

        private string? GetText()
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
    }
}
