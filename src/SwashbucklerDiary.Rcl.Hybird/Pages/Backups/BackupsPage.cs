using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    [Route("/backups")]
    public class BackupsPage : BackupsPageBase
    {
        protected override Type WebDAVBackupsPageType => typeof(WebDAVBackupsPage);
    }
}
