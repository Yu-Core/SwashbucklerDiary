using BlazorComponent.I18n;

namespace SwashbucklerDiary.Extend
{
    public static class DateTimeExtend
    {
        public static string ToWeek(this DateTime dateTime, I18n i18n)
        {
            return i18n.T("Week." + ((int)dateTime.DayOfWeek).ToString())!;
        }
    }
}
