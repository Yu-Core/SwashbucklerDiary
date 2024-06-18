using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Pages
{
    public abstract class BackupsPageBase : ImportantComponentBase
    {
        protected readonly List<TabListItem> tabListItems =
        [
            new("Backups.Local.Name","local"),
            new("Backups.WebDAV.Name","webDAV"),
        ];

        protected StringNumber tab = 0;
    }
}
