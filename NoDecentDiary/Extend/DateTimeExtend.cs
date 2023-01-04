using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Extend
{
    public static class DateTimeExtend
    {
        public enum DayOfWeekChinese1
        {
            周日,
            周一,
            周二,
            周三,
            周四,
            周五,
            周六,
        }
        public enum DayOfWeekChinese2
        {
            星期日,
            星期一,
            星期二,
            星期三,
            星期四,
            星期五,
            星期六,
        }

        public static string ToChinese1(this DayOfWeek dayOfWeek)
        {
            return ((DayOfWeekChinese1)dayOfWeek).ToString();
        }
        public static string ToChinese2(this DayOfWeek dayOfWeek)
        {
            return ((DayOfWeekChinese2)dayOfWeek).ToString();
        }
    }
}
