namespace SwashbucklerDiary.Shared
{
    public class DateFilterForm
    {
        public string DefaultDate { get; set; } = string.Empty;

        public DateOnly MinDate { get; set; } = DateOnly.MinValue;

        public DateOnly MaxDate { get; set; } = DateOnly.MaxValue;


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

            if (MinDate != DateOnly.MinValue)
            {
                return MinDate;
            }

            return DateOnly.MinValue;
        }

        public DateOnly GetDateMaxValue()
        {
            if (MaxDate != DateOnly.MaxValue)
            {
                return MaxDate;
            }

            return DateOnly.MaxValue;
        }
    }
}
