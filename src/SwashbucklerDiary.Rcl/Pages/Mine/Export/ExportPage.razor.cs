using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ExportPage : ImportantComponentBase
    {
        private StringNumber tab = 0;

        private readonly List<string> views = new() { "Local", "LAN" };

        [Parameter]
        [SupplyParameterFromQuery]
        public string? View { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitTab();
        }

        private void InitTab()
        {
            if (string.IsNullOrEmpty(View))
            {
                View = views[0];
            }
            tab = views.IndexOf(View!);
        }
    }
}
