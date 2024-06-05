using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public abstract class ExportPageBase : ImportantComponentBase
    {
        protected StringNumber tab = 0;

        protected readonly List<string> views = ["Local", "LAN"];

        protected readonly List<string> tabNames = ["Export.Local.Name", "Export.LAN.Name"];

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
