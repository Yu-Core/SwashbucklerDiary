using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Extend
{
    public static class NavigationManagerExtend
    {
        public static string ToHistoryHref(this NavigationManager navigation)
        {
            return navigation.ToBaseRelativePath(navigation.Uri).Replace("&", "%26");
        }
        public static string ToHistoryHref(this NavigationManager navigation,string uri)
        {
            return navigation.ToBaseRelativePath(uri).Replace("&", "%26");
        }
    }
}
