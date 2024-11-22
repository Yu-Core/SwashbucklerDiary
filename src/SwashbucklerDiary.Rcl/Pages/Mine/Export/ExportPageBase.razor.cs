using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Pages
{
    public abstract partial class ExportPageBase : ImportantComponentBase
    {
        protected readonly List<TabListItem> tabListItems =
        [
            new("Export.Local.Name","local"),
            new("Export.LAN.Name","LAN"),
        ];

        protected StringNumber tab = 0;

        protected abstract Type LANTransferPageType { get; }
    }
}
