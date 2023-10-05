using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagStatisticsCard
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public List<TagModel> Tags { get; set; } = new();

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();

        private int TagCount => Tags.Count;

        private int DiaryCount => Diaries.Count(d => d.Tags != null && d.Tags.Any());

        private string? EarliestDate => GetEarliestDate(Tags);

        private string? LastDate => GetLastDate(Tags);

        private static string? GetEarliestDate(List<TagModel> tags)
        {
            var earliestDate = tags.OrderBy(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }

        private static string? GetLastDate(List<TagModel> tags)
        {
            var earliestDate = tags.OrderByDescending(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }
    }
}
