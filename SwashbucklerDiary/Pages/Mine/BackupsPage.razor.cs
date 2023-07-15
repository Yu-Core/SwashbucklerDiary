using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class BackupsPage : PageComponentBase
    {
        private StringNumber tab = 0;
        private readonly List<string> Views = new() { "Local", "WebDAV" };

        [Parameter]
        [SupplyParameterFromQuery]
        public string? View { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitTab();
            SetCurrentUrl();
        }
        private void InitTab()
        {
            if (string.IsNullOrEmpty(View))
            {
                View = Views[0];
            }
            tab = Views.IndexOf(View!);
        }

        private void SetCurrentUrl()
        {
            NavigateService.SetCurrentUrl(() => {
                return Navigation.GetUriWithQueryParameter("View", Views[tab.ToInt32()]);
            });
        }

    }
}
