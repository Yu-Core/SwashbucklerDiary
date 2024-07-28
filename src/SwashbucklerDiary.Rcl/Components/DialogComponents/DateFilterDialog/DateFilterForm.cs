namespace SwashbucklerDiary.Rcl.Components
{
    public class DateFilterForm
    {
        public string DefaultDate { get; set; } = string.Empty;

        public DateOnly MinDate { get; set; }

        public DateOnly MaxDate { get; set; }

        public static readonly Dictionary<string, DateOnly> DefaultDates = new()
        {
            { "Filter.Last month", DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)) },
            { "Filter.Last three months", DateOnly.FromDateTime(DateTime.Now.AddMonths(-3)) },
            { "Filter.Last six months", DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)) },
        };

        public DateOnly GetDateMinValue()
        {
            string? defaultDate = DefaultDate?.ToString();
            if (!string.IsNullOrEmpty(defaultDate))
            {
                return DefaultDates[defaultDate];
            }

            return MinDate;
        }

        public DateOnly GetDateMaxValue()
        {
            return MaxDate;
        }
    }
}
