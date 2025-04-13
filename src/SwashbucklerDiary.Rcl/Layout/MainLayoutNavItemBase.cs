using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract class MainLayoutNavItemBase : ComponentBase
    {
        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public NavigationButton Value { get; set; } = default!;

        protected bool Activated => NavigationManager.GetAbsolutePath() == NavigationManager.ToAbsoluteUri(Value.Href).AbsolutePath;

        protected string IconClass => Activated ? "material-symbols_active" : "";
    }
}
