namespace SwashbucklerDiary.Shared
{
    public static class TimeSpanExtensions
    {
        public static string ToDurationString(this TimeSpan value)
        {
            return string.Format("{0}{1:D2}:{2:D2}",
                (int)value.TotalHours > 0 ? $"{(int)value.TotalHours}:" : "",
                value.Minutes,
                value.Seconds);
        }
    }
}
