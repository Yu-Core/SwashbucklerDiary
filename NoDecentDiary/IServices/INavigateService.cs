using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.IServices
{
    public interface INavigateService
    {
        NavigationManager? Navigation { get; set; }
        List<string> HistoryHref { get; protected set; }
        event Action Action;
        void NavigateTo(string url);
        void NavigateToBack();
        void UpdateLastHistoryHref(string href);
        void OnBackButtonPressed();
    }
}
