using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Pages;

namespace SwashbucklerDiary.Rcl.Web.Pages
{
    [Route("/export")]
    public class ExportPage : ExportPageBase
    {
        protected override Type LANTransferPageType => typeof(LANTransferPage);
    }
}
