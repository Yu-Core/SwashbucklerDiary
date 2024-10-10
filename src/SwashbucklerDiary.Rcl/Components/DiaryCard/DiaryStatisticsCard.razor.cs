using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryStatisticsCard : MyComponentBase
    {
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IGlobalConfiguration IGlobalConfiguration { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Value
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

        private int DiaryCount => Value.Count;

        private int WordCount => GetComputedValue(() => GetWordCount(Value), [nameof(Value)]);

        private int ActiveDayCount
            => GetComputedValue(() =>
            {
                return Value.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
            }
            , [nameof(Value)]);

        private string? MostWeather
            => GetComputedValue(() =>
            {
                return Value.Where(d => !string.IsNullOrWhiteSpace(d.Weather))
                            .GroupBy(d => d.Weather)
                            .OrderByDescending(d => d.Count())
                            .Select(d => d.Key)
                            .FirstOrDefault();
            }
            , [nameof(Value)]);

        private string? MostWeatherText => MostWeather is null ? I18n.T("Statistics.Not have") : I18n.T("Weather." + MostWeather);

        private string? MostWeatherIcon => MostWeather is null ? string.Empty : IGlobalConfiguration.GetWeatherIcon(MostWeather);

        private string? MostMood
            => GetComputedValue(() =>
            {
                return Value.Where(d => !string.IsNullOrWhiteSpace(d.Mood))
                            .GroupBy(d => d.Mood)
                            .OrderByDescending(d => d.Count())
                            .Select(d => d.Key)
                            .FirstOrDefault();
            }
            , [nameof(Value)]);

        private string? MostMoodText => MostMood is null ? I18n.T("Statistics.Not have") : I18n.T("Mood." + MostMood);

        private string? MostMoodIcon => MostMood is null ? string.Empty : IGlobalConfiguration.GetMoodIcon(MostMood);

        private string? EarliestDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);

        private string? LastDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);

        private int GetWordCount(List<DiaryModel> diaries)
        {
            var type = (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);
            return diaries.GetWordCount(type);
        }
    }
}
