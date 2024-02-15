using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LocationStatisticsCard
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public List<LocationModel> Value { get; set; } = new();

        private int LocationCount => Value.Count;

        private string? EarliestDate
            => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("yyyy-MM-dd");

        private string? LastDate
            => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("yyyy-MM-dd");
    }
}
