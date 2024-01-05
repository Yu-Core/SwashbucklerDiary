using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCard : MyComponentBase
    {
        private string? title;

        private string? text;

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
            LoadView();
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            title = GetTitle();
            text = GetText();
            base.OnParametersSet();
        }

        private bool HasTitle => !string.IsNullOrWhiteSpace(Value?.Title);

        private bool HasContent => !string.IsNullOrWhiteSpace(Value?.Content);

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
                new(this, PrivateText, PrivateIcon, MovePrivacy,()=>ShowPrivacy)
            };
        }

        private Task Topping()
        {
            showMenu = false;
            return DiaryCardList.Topping(Value);
        }

        private void Delete()
        {
            showMenu = false;
            DiaryCardList.Delete(Value);
        }

        private Task Copy()
        {
            showMenu = false;
            return DiaryCardList.Copy(Value);
        }

        private Task ChangeTag()
        {
            showMenu = false;
            return DiaryCardList.ChangeTag(Value);
        }

        private Task Export()
        {
            showMenu = false;
            return DiaryCardList.Export(Value);
        }

        private Task MovePrivacy()
        {
            showMenu = false;
            return DiaryCardList.MovePrivacy(Value);
        }

        private string? GetTitle()
        {
            if (HasTitle)
            {
                return Value.Title!;
            }

            if (HasContent)
            {
                char[] separators = { ',', '，', '.', '。', '?', '？', '!', '！', ';', '；', '\n' }; // 定义分隔符
                int index = Value.Content!.IndexOfAny(separators);
                if (index > -1)
                {
                    return Value.Content!.Substring(0, index + 1);
                }
                else
                {
                    return SubText(Value.Content, 0, 200);
                }
            }

            return null;
        }

        private string? GetText()
        {
            string text = SubText(Value.Content, 0, 2000);
            if (!HasTitle && title is not null && HasContent)
            {
                var subText = SubText(text, title.Length);
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

        private void ToRead()
        {
            NavigateService.PushAsync($"read?Id={Value.Id}");
        }
    }
}
