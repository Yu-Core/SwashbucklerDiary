using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagStatisticsCard
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public List<TagModel> Value { get; set; } = [];

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = [];

        private int TagCount => Value.Count;

        private int DiaryCount => Diaries.Count(d => d.Tags is not null && d.Tags.Count != 0);

        private string? EarliestDate
            => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("d");

        private string? LastDate
            => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("d");
    }
}
