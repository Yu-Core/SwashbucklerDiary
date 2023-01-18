using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
{
    public class NavigateService : INavigateService
    {
        NavigationManager _navigation { get; set; }
        public NavigateService(NavigationManager navigation) 
        {
            _navigation = navigation;
        }
        public List<string> HistoryHref { get; set; } = new List<string>();

        public void NavigateTo(string url)
        {
            var href = _navigation.ToBaseRelativePath(_navigation.Uri);
            HistoryHref.Add(href);
            _navigation.NavigateTo(url);
        }
        public void NavigateToBack()
        {
            string href = string.Empty;
            if (HistoryHref.Count > 0)
            {
                href = HistoryHref.Last();
            }
            _navigation.NavigateTo(href);
            if (HistoryHref.Count > 0)
            {
                HistoryHref.RemoveAt(HistoryHref.Count - 1);
            }
        }
        public void UpdateLastHistoryHref(string href)
        {
            if (HistoryHref.Count > 0)
            {
                HistoryHref.RemoveAt(HistoryHref.Count - 1);
            }
            HistoryHref.Add(href);
        }
    }
}
