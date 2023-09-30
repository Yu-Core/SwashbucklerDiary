using BlazorComponent;
using BlazorComponent.Attributes;
using Masa.Blazor.Presets;
using Masa.Blazor.Presets.PageContainer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Components
{
    public partial class PPageContainerReplacement : PatternPathComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// A list of regular expression patterns to match.
        /// The content of the matched path would not be cached in the DOM.
        /// </summary>
        [Parameter]
        public IEnumerable<string>? ExcludePatterns { get; set; }

        /// <summary>
        /// A list of regular expression patterns to match.
        /// The content of the matched path would be cached in the DOM.
        /// </summary>
        [Parameter]
        public IEnumerable<string>? IncludePatterns { get; set; }

        /// <summary>
        /// Max number of cached pages.
        /// </summary>
        [Parameter]
        [ApiDefaultValue(10)]
        public int Max { get; set; } = 10;

        protected readonly LRUCache<string, PatternPath> _patternPaths = new(10);

        private string? _previousPath;

        private PatternPath? _currentPatternPath;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var patternPath = GetCurrentPatternPath();
            _currentPatternPath = patternPath;
            _patternPaths.Put(patternPath.Pattern, patternPath);

            _previousPath = patternPath.AbsolutePath;

            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected override void SetComponentClass()
        {
            base.SetComponentClass();

            CssProvider.Apply(css => { css.Add("p-page-container"); }, style => { style.Add("height: 100%"); });
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            var currentPatternPath = GetCurrentPatternPath();
            var currentPath = currentPatternPath.AbsolutePath;

            // only the path is changed, not the query string
            if (_previousPath == currentPath)
            {
                return;
            }

            if (currentPatternPath.IsSelf)
            {
                if (_patternPaths.TryGetValue(currentPath, out var renderedPathPattern))
                {
                    renderedPathPattern.UpdatePath(NavigationManager.GetAbsolutePath());
                    InvokeAsync(StateHasChanged);
                }
            }

            // if the previous path is excluded or not included, remove it from the PatternPaths
            if (_previousPath is not null)
            {
                if ((ExcludePatterns?.Any() is true &&
                     ExcludePatterns.Any(pattern => new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(_previousPath))) ||
                    (IncludePatterns?.Any() is true &&
                     !IncludePatterns.Any(pattern => new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(_previousPath))))
                {
                    var previousPatternPath = _patternPaths.FirstOrDefault(p => p.AbsolutePath == _previousPath);
                    if (previousPatternPath is not null)
                    {
                        _patternPaths.Remove(previousPatternPath.Pattern);
                        InvokeAsync(StateHasChanged);
                    }
                }
            }

            _previousPath = currentPath;

            _currentPatternPath = currentPatternPath;

            _patternPaths.PutOrGet(currentPatternPath.Pattern, currentPatternPath);

            InvokeAsync(StateHasChanged);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
        }
    }
}
