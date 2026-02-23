using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Pages;

namespace SwashbucklerDiary.Rcl.Web.Pages
{
    [Route("/backups")]
    public class BackupsPage : BackupsPageBase
    {
        protected override Type WebDAVBackupsPageType => typeof(WebDAVBackupsPage);
    }
}
