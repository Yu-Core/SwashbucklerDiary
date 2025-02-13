using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    [Route("/export")]
    public class ExportPage : ExportPageBase
    {
        protected override Type LANTransferPageType => typeof(LANTransferPage);
    }
}
