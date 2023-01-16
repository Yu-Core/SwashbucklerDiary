using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Extend
{
    public static class StringExtend
    {
        public static string ToHistoryHref(this string str)
        {
            return str.Replace("&", "%26");
        }
    }
}
