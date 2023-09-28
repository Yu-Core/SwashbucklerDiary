using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public partial class PPageContainerExtension : PPageContainerReplacement
    {
        [Inject]
        private INavigateService NavigateService { get; set; } = default!;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            NavigateService.Pushed -= Pushed;
            NavigateService.Poped -= Poped;
            NavigateService.PopedToRoot -= PopedToRoot;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateService.Pushed += Pushed;
            NavigateService.Poped += Poped;
            NavigateService.PopedToRoot += PopedToRoot;
        }

        private void Poped(PopEventArgs e)
        {
            Remove(e.NextUri);
            InvokeAsync(StateHasChanged);
        }

        private void Pushed(PushEventArgs e)
        {
            if (!e.IsCachePrevious)
            {
                Remove(e.PreviousUri);
            }

            InvokeAsync(StateHasChanged);
        }

        private void PopedToRoot(PopEventArgs e)
        {
            var rootPaths = NavigateService.RootPaths.Select(it => new Uri(it).AbsolutePath);
            var paths = _patternPaths.Where(p => rootPaths.Contains(p.AbsolutePath))
                .Select(p => p.AbsolutePath)
                .ToArray();
            _patternPaths.RemoveAll(_patternPaths.Keys.Except(paths));
            InvokeAsync(StateHasChanged);
        }

        private void Remove(string url)
        {
            var absolutePath = new Uri(url).AbsolutePath;
            var pattern = _patternPaths.FirstOrDefault(p => p.AbsolutePath == absolutePath);
            if (pattern is null)
            {
                return;
            }

            _patternPaths.Remove(pattern.Pattern);
        }
    }
}
