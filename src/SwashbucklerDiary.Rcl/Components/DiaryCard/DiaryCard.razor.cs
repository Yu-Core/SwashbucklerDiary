using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
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

        private DiaryModel? previousValue;

        [Inject]
        private IIconService IconService { get; set; } = default!;

        [CascadingParameter]
        public DiaryCardListOptions DiaryCardListOptions { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            SetContent();
        }

        private bool IsActive => Value.Id == DiaryCardListOptions.SelectedItemValue.Id;

        private string? TimeFormat => DiaryCardListOptions.TimeFormat;

        private string? Date => TimeFormat is null ? null : Value.CreateTime.ToString(TimeFormat);

        private string Theme => Dark ? "theme--dark" : "theme--light";

        private void ToRead()
        {
            NavigationManager.NavigateTo($"read/{Value.Id}");
        }

        private void SetContent()
        {
            if (previousValue != Value)
            {
                previousValue = Value;
                title = Value.ExtractTitle();
                text = Value.ExtractText();
                weatherIcon = string.IsNullOrWhiteSpace(Value.Weather) ? null : IconService.GetWeatherIcon(Value.Weather);
                moodIcon = string.IsNullOrWhiteSpace(Value.Mood) ? null : IconService.GetMoodIcon(Value.Mood);
            }
        }
    }
}
