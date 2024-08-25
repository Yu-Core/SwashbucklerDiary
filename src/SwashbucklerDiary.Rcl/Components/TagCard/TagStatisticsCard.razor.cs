using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagStatisticsCard : MyComponentBase
    {
        [Parameter]
        public List<TagModel> Value
        {
            get => GetValue<List<TagModel>>() ?? [];
            set => SetValue(value);
        }

        [Parameter]
        public List<DiaryModel> Diaries
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

        private int TagCount => Value.Count;

        private int DiaryCount
            => GetComputedValue(() => Diaries.Count(d => d.Tags is not null && d.Tags.Count != 0), [nameof(Diaries)]);

        private string? EarliestDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);

        private string? LastDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);
    }
}
