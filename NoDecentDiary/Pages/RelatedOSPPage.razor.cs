using Microsoft.AspNetCore.Components;
using NoDecentDiary.Components;
using NoDecentDiary.Shared;

namespace NoDecentDiary.Pages
{
    public partial class RelatedOSPPage : PageComponentBase
    {
        private List<OSP> OSPs = new()
        {
            new OSP(".NET MAUI","MIT license","https://github.com/dotnet/maui"),
            new OSP("ASP.NET Core","MIT license","https://github.com/dotnet/aspnetcore"),
            new OSP("html2canvas","MIT license","https://github.com/niklasvh/html2canvas"),
            new OSP("MASA.Blazor","MIT license","https://github.com/BlazorComponent/MASA.Blazor"),
            new OSP("Serilog","Apache-2.0 license","https://github.com/serilog/serilog"),
            new OSP("SQLite-net","MIT license","https://github.com/praeclarum/sqlite-net"),
            new OSP("Swiper","MIT license","https://github.com/nolimits4web/swiper"),
        };
        private class OSP
        {
            public OSP(string name,string license,string url) 
            { 
                Name = name;
                License = license;
                Url = url;
            }
            public string Name { get; set; }
            public string License { get; set; }
            public string Url { get; set; }
        }

        [CascadingParameter]
        public Error Error { get; set; } = default!;

        private async Task OpenBrowser(string url)
        {
            try
            {
                await SystemService.OpenBrowser(url);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
                Error.ProcessError(ex);
            }
        }
    }
}
