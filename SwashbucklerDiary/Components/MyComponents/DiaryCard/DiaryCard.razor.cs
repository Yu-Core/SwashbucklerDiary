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

        [CascadingParameter]
        public DiaryCardList DiaryCardList { get; set; } = default!;
        [Parameter]
        public DiaryModel Value { get; set; } = default!;
        [Parameter]
        public string? Class { get; set; }

        protected override void OnInitialized()
        {
            LoadView();
            SetContent();
            base.OnInitialized();
        }

        private bool HasTitle => !string.IsNullOrWhiteSpace(Value?.Title);
        private bool HasContent => !string.IsNullOrWhiteSpace(Value?.Content);
        private DateTime Time => Value.CreateTime;
        private bool IsTop => Value.Top;
        private bool IsPrivate => Value.Private;
        private bool ShowPrivacy => DiaryCardList.ShowPrivacy;
        private bool ShowIcon => DiaryCardList.ShowIcon;
        private string? DateFormat => DiaryCardList.DateFormat;

        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";
        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";
        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this,"Diary.Tag","mdi-label-outline",ChangeTag),
                new(this, "Share.Copy","mdi-content-copy",Copy),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
                new(this, TopText,"mdi-format-vertical-align-top",Topping),
                new(this, "Diary.Export","mdi-export",Export)
            };

            if (ShowPrivacy)
            {
                ListItemModels.Add(new(this, PrivateText, PrivateIcon, MovePrivacy));
            }
        }

        private string? GetWeatherIcon() =>
            string.IsNullOrWhiteSpace(Value.Weather) ? null : IconService.GetWeatherIcon(Value.Weather);

        private string? GetMoodIcon() =>
            string.IsNullOrWhiteSpace(Value.Mood) ? null : IconService.GetMoodIcon(Value.Mood);

        private void SetContent()
        {
            Title = GetTitle();
            Text = GetText();
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
            }

            return null;
        }

        private string? GetText()
        {
            string text = SubText(Value.Content, 0, 1000);
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

        private void ToRead()
        {
            NavigateService.NavigateTo($"/read/{Value.Id}");
        }

        private Task Topping() => DiaryCardList.Topping(Value);

        private void Delete() => DiaryCardList.Delete(Value);

        private Task Copy() => DiaryCardList.Copy(Value);

        private Task ChangeTag() => DiaryCardList.ChangeTag(Value);

        private void Export() => DiaryCardList.Export(Value);

        private Task MovePrivacy() => DiaryCardList.MovePrivacy(Value);
    }
}
