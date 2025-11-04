using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCard : CardComponentBase<DiaryModel>
    {
        private string? text;

        private string? weatherIcon;

        private string? moodIcon;

        private DiaryModel? previousValue;

        private Dictionary<string, object>? _markdownOptions;

        [Inject]
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [CascadingParameter]
        public DiaryCardListOptions DiaryCardListOptions { get; set; } = default!;

        [Parameter]
        public EventCallback<DiaryModel> OnClick { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _markdownOptions = new()
            {
                ["markdown"] = new Dictionary<string, object?>()
                {
                    ["linkBase"] = MediaResourceManager.MarkdownLinkBase,
                    ["imgPathAllowSpace"] = true
                }
            };
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            SetContent();
        }

        private string MenuIconClass => new CssBuilder()
            .Add("card__menu")
            .Add("card-list__selected-menu", IsActive)
            .ToString();

        private bool IsActive => Value.Id == DiaryCardListOptions.SelectedItemValue.Id;

        private string? TimeFormat => DiaryCardListOptions.TimeFormat;

        private bool Markdown => DiaryCardListOptions.Markdown;

        private void SetContent()
        {
            if (previousValue != Value)
            {
                previousValue = Value;
                text = Value.GetDisplayContent();
                weatherIcon = string.IsNullOrWhiteSpace(Value.Weather) ? null : GlobalConfiguration.GetWeatherIcon(Value.Weather);
                moodIcon = string.IsNullOrWhiteSpace(Value.Mood) ? null : GlobalConfiguration.GetMoodIcon(Value.Mood);
            }
        }

        private async Task HandeOnClick()
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(Value);
            }
        }
    }
}
