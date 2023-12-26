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

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();

        private int DiaryCount => Diaries.Count;

        private int WordCount => GetWordCount(Diaries);

        private int ActiveDayCount
            => Diaries.Select(it => DateOnly.FromDateTime(it.CreateTime))
            .Distinct()
            .Count();

        private string? MostWeather
            => Diaries.Where(d => !string.IsNullOrWhiteSpace(d.Weather))
            .GroupBy(d => d.Weather)
            .OrderByDescending(d => d.Count())
            .Select(d => d.Key)
            .FirstOrDefault();

        private string? MostWeatherText => MostWeather is null ? I18n.T("Statistics.Not have") : I18n.T("Weather." + MostWeather);

        private string? MostWeatherIcon => MostWeather is null ? string.Empty :  IIconService.GetWeatherIcon(MostWeather);

        private string? MostMood
            => Diaries.Where(d => !string.IsNullOrWhiteSpace(d.Mood))
            .GroupBy(d => d.Mood)
            .OrderByDescending(d => d.Count())
            .Select(d => d.Key)
            .FirstOrDefault();

        private string? MostMoodText => MostMood is null ? I18n.T("Statistics.Not have") : I18n.T("Mood." + MostMood);

        private string? MostMoodIcon => MostMood is null ? string.Empty : IIconService.GetMoodIcon(MostMood);

        private string? EarliestDate => GetEarliestDate(Diaries);

        private string? LastDate => GetLastDate(Diaries);

        private int GetWordCount(List<DiaryModel> diaries)
        {
            var type = (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);
            return DiaryService.GetWordCount(diaries, type);
        }

        private static string? GetEarliestDate(List<DiaryModel> diaries)
        {
            var earliestDate = diaries.OrderBy(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }

        private static string? GetLastDate(List<DiaryModel> diaries)
        {
            var earliestDate = diaries.OrderByDescending(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }
    }
}
