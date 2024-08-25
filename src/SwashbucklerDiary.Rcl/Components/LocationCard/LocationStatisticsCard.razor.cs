using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LocationStatisticsCard : MyComponentBase
    {
        [Parameter]
        public List<LocationModel> Value
        {
            get => GetValue<List<LocationModel>>() ?? [];
            set => SetValue(value);
        }

        private int LocationCount => Value.Count;

        private string? EarliestDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).FirstOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);

        private string? LastDate
            => GetComputedValue(() => Value.OrderBy(d => d.CreateTime).LastOrDefault()?.CreateTime.ToString("d"), [nameof(Value)]);
    }
}
