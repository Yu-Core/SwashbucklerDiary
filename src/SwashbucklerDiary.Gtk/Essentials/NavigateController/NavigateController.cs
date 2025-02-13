using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;
using Application = Gtk.Application;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override Assembly[] Assemblies { get; } = [Routes.Assembly, .. Routes.AdditionalAssemblies];

        protected override Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            context.PreventNavigation();
            Application.GetDefault()?.Quit();
            return Task.CompletedTask;
        }
    }
}
