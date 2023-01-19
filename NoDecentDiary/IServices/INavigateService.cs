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
        public NavigationManager? Navigation { get; set; }
        List<string> HistoryHref { get; protected set; }
        public event Action Action;
        public void NavigateTo(string url);
        public void NavigateToBack();
        public void UpdateLastHistoryHref(string href);
        public void OnBackButtonPressed();
    }
}
