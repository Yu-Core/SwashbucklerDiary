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
        public List<LocationModel> Locations { get; set; } = new();

        private int LocationCount => Locations.Count;

        private string? EarliestDate => GetEarliestDate(Locations);

        private string? LastDate => GetLastDate(Locations);

        private static string? GetEarliestDate(List<LocationModel> locations)
        {
            var earliestDate = locations.OrderBy(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }

        private static string? GetLastDate(List<LocationModel> locations)
        {
            var earliestDate = locations.OrderByDescending(d => d.CreateTime).FirstOrDefault();
            return earliestDate is null ? string.Empty : earliestDate.CreateTime.ToString("yyyy-MM-dd");
        }
    }
}
