using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class ExportPage : ImportantComponentBase
    {
        private StringNumber tab = 0;
        private readonly List<string> Views = new() { "Local", "LAN" };

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
                View = Views[0];
            }
            tab = Views.IndexOf(View!);
        }
    }
}
