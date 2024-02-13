namespace SwashbucklerDiary.Shared
{
    public static class DateOnlyExtensions
    {
        public static DateTime ToDateTime(this DateOnly dateOnly)
        {
            return dateOnly.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
        }
    }
}
