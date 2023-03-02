using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.IServices
{
    public interface INavigateService
    {
        event Action Action;
        NavigationManager Navigation { get; set; }
        List<string> HistoryUrl { get; protected set; }
        void NavigateTo(string url);
        void NavigateToBack();
        void OnBackButtonPressed();
    }
}
