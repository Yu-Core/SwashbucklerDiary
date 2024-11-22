using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Pages;

namespace SwashbucklerDiary.WebAssembly.Pages
{
    [Route("/export")]
    public class ExportPage : ExportPageBase
    {
        protected override Type LANTransferPageType => typeof(LANTransferPage);
    }
}
