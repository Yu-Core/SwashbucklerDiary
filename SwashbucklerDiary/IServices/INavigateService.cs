using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.IServices
{
    public interface INavigateService
    {
        NavigationManager Navigation { get; set; }
        List<string> HistoryHref { get; protected set; }
        event Action Action;
        void NavigateTo(string url);
        void NavigateToBack();
        void UpdateLastHistoryHref(string href);
        void OnBackButtonPressed();
    }
}
