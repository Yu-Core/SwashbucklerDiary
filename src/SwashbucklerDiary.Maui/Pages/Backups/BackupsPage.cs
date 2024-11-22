using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Pages;

namespace SwashbucklerDiary.Maui.Pages
{
    [Route("/backups")]
    public class BackupsPage : BackupsPageBase
    {
        protected override Type WebDAVBackupsPageType => typeof(WebDAVBackupsPage);
    }
}
