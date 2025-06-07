using BemIt;
using Masa.Blazor;
using Masa.Blazor.Attributes;
using Masa.Blazor.Presets.PageContainer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PPageContainerReplacement : PatternPathComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// A list of regular expression patterns to match.
        /// The content of the matched path would not be cached in the DOM.
        /// </summary>
        [Parameter]
        public IEnumerable<string> ExcludePatterns { get; set; } = Array.Empty<string>();

        /// <summary>
        /// A list of regular expression patterns to match.
        /// The content of the matched path would be cached in the DOM.
        /// </summary>
        [Parameter]
        [MasaApiParameter(ReleasedIn = "v1.0.2")]
        public IEnumerable<string> IncludePatterns { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Max number of cached pages.
        /// </summary>
        [Parameter]
        [MasaApiParameter(10, "v1.0.2")]
        public int Max { get; set; } = 10;

        [Parameter] public string? Transition { get; set; } = "";

        /// <summary>
        /// Strict mode, only the patterns in <see cref="IncludePatterns"/>
        /// and <see cref="PatternPathComponentBase.SelfPatterns"/> would be rendered.
        /// </summary>
        [Parameter]
        public bool Strict { get; set; }

        [Parameter]
        public bool UseRegex { get; set; } = true;

        [Parameter]
        public bool PageUpdate { get; set; } = true;

        protected readonly LRUCache<string, PatternPath> _patternPaths = new(10);

        private readonly Block _block = new("p-page-container");

        protected string? _previousPath;
        private PatternPath? _currentPatternPath;

        private HashSet<string> _prevIncludePatterns = [];
        private HashSet<string> _prevExcludePatterns = [];

        private HashSet<Regex> _cachedIncludePatternRegexes = [];
        private HashSet<Regex> _cachedExcludePatternRegexes = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateCacheRegexes();

            var patternPath = GetCurrentPatternPath();

            if (!Strict || _cachedIncludePatternRegexes.Any(r => IsMatch(r, patternPath.AbsolutePath)))
            {
                _currentPatternPath = patternPath;
                _patternPaths.Put(patternPath.Pattern, patternPath);
                _previousPath = patternPath.AbsolutePath;
            }

            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            UpdateCacheRegexes();
        }

        private void UpdateCacheRegexes()
        {
            if (!_prevIncludePatterns.SetEquals(IncludePatterns))
            {
                _prevIncludePatterns = new HashSet<string>(IncludePatterns);
                _cachedIncludePatternRegexes =
                    IncludePatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToHashSet();
            }

            if (!_prevExcludePatterns.SetEquals(ExcludePatterns))
            {
                _prevExcludePatterns = new HashSet<string>(ExcludePatterns);
                _cachedExcludePatternRegexes =
                    ExcludePatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToHashSet();
            }
        }

        protected override IEnumerable<string> BuildComponentClass()
        {
            yield return "p-page-container";
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!PageUpdate)
            {
                return;
            }

            var currentPath = NavigationManager.GetAbsolutePath();
            if (Strict && !_cachedIncludePatternRegexes.Any(r => IsMatch(r, currentPath)))
            {
                return;
            }

            // only the path is changed, not the query string
            if (_previousPath == currentPath)
            {
                return;
            }

            var currentPatternPath = GetCurrentPatternPath();

            if (currentPatternPath.IsSelf)
            {
                if (_patternPaths.TryGetValue(currentPatternPath.Pattern, out var renderedPathPattern))
                {
                    renderedPathPattern.UpdatePath(NavigationManager.GetAbsolutePath());
                    InvokeAsync(StateHasChanged);
                }
            }

            // if the previous path is excluded or not included, remove it from the PatternPaths
            if (_previousPath is not null && (
                    (_cachedExcludePatternRegexes.Count > 0 &&
                     _cachedExcludePatternRegexes.Any(r => IsMatch(r, _previousPath))) ||
                    (_cachedIncludePatternRegexes.Count > 0 &&
                     !_cachedIncludePatternRegexes.Any(r => IsMatch(r, _previousPath))))
               )
            {
                var previousPatternPath = _patternPaths.FirstOrDefault(p => p.AbsolutePath == _previousPath);
                if (previousPatternPath is not null)
                {
                    _patternPaths.Remove(previousPatternPath.Pattern);
                    InvokeAsync(StateHasChanged);
                }
            }

            _previousPath = currentPath;

            _currentPatternPath = currentPatternPath;

            _patternPaths.PutOrGet(currentPatternPath.Pattern, currentPatternPath);

            InvokeAsync(StateHasChanged);
        }

        private bool IsMatch(Regex regex, string input)
        {
            return UseRegex ? regex.IsMatch(input) : regex.ToString() == input;
        }

        protected override ValueTask DisposeAsyncCore()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;

            return base.DisposeAsyncCore();
        }
    }
}
