using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class RelatedOSPPage : PageComponentBase
    {
        private readonly List<OSP> OSPs = new()
        {
            new OSP(".NET MAUI","MIT license","https://github.com/dotnet/maui/blob/main/LICENSE.txt"),
            new OSP("ASP.NET Core","MIT license","https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt"),
            new OSP("CommunityToolkit.Maui","MIT license","https://github.com/CommunityToolkit/Maui/blob/main/LICENSE"),
            new OSP("html2canvas","MIT license","https://github.com/niklasvh/html2canvas/blob/master/LICENSE"),
            new OSP("Markdown Guide","MIT license","https://github.com/mattcone/markdown-guide/blob/master/LICENSE.txt"),
            new OSP("MASA.Blazor","MIT license","https://github.com/BlazorComponent/MASA.Blazor/blob/main/LICENSE"),
            new OSP("P/Invoke","MIT license","https://github.com/dotnet/pinvoke/blob/main/LICENSE"),
            new OSP("Serilog","Apache-2.0 license","https://github.com/serilog/serilog/blob/dev/LICENSE"),
            new OSP("Serilog.Sinks.SQLite.Maui","Apache-2.0 license","https://github.com/Yu-Core/Serilog-Sinks-SQLite-Maui/blob/dev/LICENSE"),
            new OSP("SQLite-net","MIT license","https://github.com/praeclarum/sqlite-net/blob/master/LICENSE.txt"),
            new OSP("SqlSugar","Apache-2.0 license","https://github.com/DotNetNext/SqlSugar/blob/master/LICENSE"),
            new OSP("Swiper","MIT license","https://github.com/nolimits4web/swiper/blob/master/LICENSE"),
            new OSP("WebDavClient","MIT license","https://github.com/skazantsev/WebDavClient/blob/main/LICENSE.txt"),
            new OSP("Vditor","MIT license","https://github.com/Vanessa219/vditor/blob/master/LICENSE"),
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
