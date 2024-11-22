using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Pages;

namespace SwashbucklerDiary.WebAssembly.Pages
{
    [Route("/backups")]
    public class BackupsPage : BackupsPageBase
    {
        protected override Type WebDAVBackupsPageType => typeof(WebDAVBackupsPage);
    }
}
