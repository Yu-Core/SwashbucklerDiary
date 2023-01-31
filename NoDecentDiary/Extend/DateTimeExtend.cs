using BlazorComponent.I18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Extend
{
    public static class DateTimeExtend
    {
        public static string ToWeek(this DateTime dateTime, I18n? i18n)
        {
            return i18n!.T("Week." + ((int)dateTime.DayOfWeek).ToString());
        }
    }
}
