using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Maui.Pages
{
    public partial class BackupsPage : ImportantComponentBase
    {
        private StringNumber tab = 0;

        private readonly List<string> views = new() { "Local", "WebDAV" };

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
