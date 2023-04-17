using BlazorComponent;

namespace SwashbucklerDiary.Models.Form
{
    public class DateFilterForm
    {
        public string DefaultDate { get; set; } = string.Empty;
        public DateOnly MinDate { get; set; } = DateOnly.MinValue;
        public DateOnly MaxDate { get; set; } = DateOnly.MaxValue;
    }
}
