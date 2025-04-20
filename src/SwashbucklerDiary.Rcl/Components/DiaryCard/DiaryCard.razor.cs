using Masa.Blazor.Core;
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
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [CascadingParameter]
        public DiaryCardListOptions DiaryCardListOptions { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public EventCallback<DiaryModel> OnClick { get; set; }

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

        private string Theme => Dark ? "theme--dark" : "theme--light";

        private void SetContent()
        {
            if (previousValue != Value)
            {
                previousValue = Value;
                title = Value.GetDisplayTitle();
                text = Value.GetDisplayContent(title?.Length ?? 0);
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
