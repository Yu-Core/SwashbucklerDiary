using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Interface
{
    public interface INavigateToBack
    {
        string? Href { get; set; }
        NavigationManager? Navigation { get; set; }
        void NavigateToBack();
    }

    public static class INavigateToBackExtend
    {
        public static void DefaultNavigateToBack(this INavigateToBack navigateToBack)
        {
            if (!string.IsNullOrEmpty(navigateToBack.Href))
            {
                navigateToBack.Navigation!.NavigateTo(navigateToBack.Href);
            }
            else
            {
                navigateToBack.Navigation!.NavigateTo("/");
            }
        }
    }

}
