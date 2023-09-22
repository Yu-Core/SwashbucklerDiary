using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class NavigateService : INavigateService
    {
        public event Action? Action;
        public event Func<PushEventArgs, Task>? BeforePush;
        public event Func<PopEventArgs, Task>? BeforePop;
        public event Func<PopEventArgs, Task>? BeforePopToRoot;
        public event Action<PushEventArgs>? Pushed;
        public event Action<PopEventArgs>? Poped;
        public event Action<PopEventArgs>? PopedToRoot;

        public string? RootPath { get; set; }
        public NavigationManager Navigation { get; set; } = default!;
        public List<string> HistoryURLs { get; set; } = new();

        public void Initialize(NavigationManager navigation)
        {
            Navigation = navigation;
        }

        public void SetRootPath()
        {
            if (RootPath is not null)
            {
                return;
            }

            RootPath = Navigation.Uri;
        }

        public async Task PushAsync(string url, bool isCachePrevious = true)
        {
            string nextUri = new Uri(new(Navigation.BaseUri), url).ToString();
            PushEventArgs args = new(Navigation.Uri, nextUri, isCachePrevious);

            if (BeforePush is not null)
            {
                await BeforePush.Invoke(args);
            }

            string currentURL = Navigation.Uri;
            HistoryURLs.Add(currentURL);

            Navigation.NavigateTo(url);
            Pushed?.Invoke(args);
        }

        public async Task PopAsync()
        {
            if (HistoryURLs.Count > 0)
            {
                string previousUri = HistoryURLs.Last();
                PopEventArgs args = new(previousUri, Navigation.Uri);

                if (BeforePop is not null)
                {
                    await BeforePop.Invoke(args);
                }

                var lastIndex = HistoryURLs.Count - 1;
                HistoryURLs.RemoveAt(lastIndex);
                Navigation.NavigateTo(previousUri);

                Poped?.Invoke(args);
            }
        }

        public async Task PopToRootAsync()
        {
            if (RootPath is null)
            {
                return;
            }

            var current = new Uri(Navigation.Uri).AbsolutePath;
            if (current == new Uri(RootPath).AbsolutePath)
            {
                return;
            }

            PopEventArgs args = new(RootPath, Navigation.Uri);
            if (BeforePopToRoot is not null)
            {
                await BeforePopToRoot.Invoke(args);
            }

            Navigation.NavigateTo(RootPath);
            HistoryURLs.Clear();
            PopedToRoot?.Invoke(args);
        }

        public bool OnBackButtonPressed()
        {
            if (Action != null && Action?.GetInvocationList().Length > 0)
            {
                var delegates = Action.GetInvocationList();
                (delegates.Last() as Action)!.Invoke();
                return true;
            }

            if (HistoryURLs.Any())
            {
                _ = PopAsync();
                return true;
            }

            return false;
        }
    }
}
