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

        private Guid previousValueId;

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

        private bool ShowIcon => DiaryCardList.ShowIcon;

        private string? DateFormat => DiaryCardList.DateFormat;

        private string? Date => DateFormat is null ? null : Value.CreateTime.ToString(DateFormat);

        private void ToRead()
        {
            NavigateService.PushAsync($"read?Id={Value.Id}");
        }

        private void SetContent()
        {
            if (previousValueId != Value.Id)
            {
                previousValueId = Value.Id;
                title = Value.ExtractTitle();
                text = Value.ExtractText();
                weatherIcon = string.IsNullOrWhiteSpace(Value.Weather) ? null : IconService.GetWeatherIcon(Value.Weather);
                moodIcon = string.IsNullOrWhiteSpace(Value.Mood) ? null : IconService.GetMoodIcon(Value.Mood);
            }
        }
    }
}
