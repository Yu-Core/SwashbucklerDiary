using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.IServices
{
    public interface INavigateService
    {
        List<string> HistoryHref { get; protected set; }
        public void NavigateTo(string url);
        public void NavigateToBack();
        public void UpdateLastHistoryHref(string href);
    }
}
