using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Pages
{
    public partial class RelatedOSPPage : PageComponentBase
    {
        private readonly List<OSP> OSPs = new()
        {
            new OSP(".NET MAUI","MIT license","https://github.com/dotnet/maui"),
            new OSP("ASP.NET Core","MIT license","https://github.com/dotnet/aspnetcore"),
            new OSP("CommunityToolkit.Maui","MIT license","https://github.com/CommunityToolkit/Maui"),
            new OSP("html2canvas","MIT license","https://github.com/niklasvh/html2canvas"),
            new OSP("Markdown Guide","MIT license","https://github.com/mattcone/markdown-guide"),
            new OSP("MASA.Blazor","MIT license","https://github.com/BlazorComponent/MASA.Blazor"),
            new OSP("Serilog","Apache-2.0 license","https://github.com/serilog/serilog"),
            new OSP("SQLite-net","MIT license","https://github.com/praeclarum/sqlite-net"),
            new OSP("SqlSugar","Apache-2.0 license","https://github.com/DotNetNext/SqlSugar"),
            new OSP("Swiper","MIT license","https://github.com/nolimits4web/swiper"),
        };
        private class OSP
        {
            public OSP(string name, string license, string url)
            {
                Name = name;
                License = license;
                Url = url;
            }
            public string Name { get; set; }
            public string License { get; set; }
            public string Url { get; set; }
        }

        private async Task OpenBrowser(string url)
        {
            await SystemService.OpenBrowser(url);
        }
    }
}
