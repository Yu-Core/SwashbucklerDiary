using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public abstract class BackupsPageBase : ImportantComponentBase
    {
        protected StringNumber tab = 0;

        protected readonly List<string> views = ["Local", "WebDAV"];

        protected readonly List<string> tabNames = ["Backups.Local.Name", "Backups.WebDAV.Name"];

        [Parameter]
        [SupplyParameterFromQuery]
        public string? View { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitTab();
        }

        protected void InitTab()
        {
            if (string.IsNullOrEmpty(View))
            {
                View = views[0];
            }

            tab = views.IndexOf(View!);
        }
    }
}
