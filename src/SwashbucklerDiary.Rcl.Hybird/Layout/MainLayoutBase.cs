using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Hybird.Extensions;

namespace SwashbucklerDiary.Rcl.Hybird.Layout
{
    public class MainLayoutBase : Rcl.Layout.MainLayoutBase
    {
        [Inject]
        protected RouteMatcher RouteMatcher { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InternalOnInitializedAsync();

            await UpdateDocumentProperty(I18n.Culture);
        }

        protected override void HandleSchemeActivation(ActivationArguments args, bool replace)
        {
            string? uriString = args?.Data as string;
            if (RouteMatcher.CheckUrlScheme(uriString, out var path))
            {
                To(path.TrimStart('/'), replace: replace);
            }
        }
    }
}
