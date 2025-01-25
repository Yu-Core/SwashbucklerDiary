using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Layout;
using System.Reflection;
using Application = Gtk.Application;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override Assembly[] Assemblies => [typeof(MainLayoutBase).Assembly, typeof(Routes).Assembly];

        protected override Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            context.PreventNavigation();
            Application.Quit();
            return Task.CompletedTask;
        }
    }
}
