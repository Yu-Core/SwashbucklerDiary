using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryStatisticsCard
    {
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IIconService IIconService { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public List<DiaryModel> Value { get; set; } = new();

        private int DiaryCount => Value.Count;

        private int WordCount => GetWordCount(Value);

        private int ActiveDayCount
            => Value.Select(it => DateOnly.FromDateTime(it.CreateTime))
            .Distinct()
            .Count();

        private string? MostWeather
            => Value.Where(d => !string.IsNullOrWhiteSpace(d.Weather))
            .GroupBy(d => d.Weather)
            .OrderByDescending(d => d.Count())
            .Select(d => d.Key)
            .FirstOrDefault();

        private string? MostWeatherText => MostWeather is null ? I18n.T("Statistics.Not have") : I18n.T("Weather." + MostWeather);

        private string? MostWeatherIcon => MostWeather is null ? string.Empty : IIconService.GetWeatherIcon(MostWeather);

        private string? MostMood
            => Value.Where(d => !string.IsNullOrWhiteSpace(d.Mood))
            .GroupBy(d => d.Mood)
            .OrderByDescending(d => d.Count())
            .Select(d => d.Key)
            .FirstOrDefault();

        private string? MostMoodText => MostMood is null ? I18n.T("Statistics.Not have") : I18n.T("Mood." + MostMood);

        private string? MostMoodIcon => MostMood is null ? string.Empty : IIconService.GetMoodIcon(MostMood);

        private string? EarliestDate
            => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("d");

        private string? LastDate
            => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("d");

        private int GetWordCount(List<DiaryModel> diaries)
        {
            var type = (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);
            return DiaryService.GetWordCount(diaries, type);
        }
    }
}
