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
            NavigateService.BeforeNavigate += SetCurrentUrl;
        }

        protected override void OnDispose()
        {
            NavigateService.BeforeNavigate -= SetCurrentUrl;
            base.OnDispose();
        }

        private void InitTab()
        {
            if (string.IsNullOrEmpty(View))
            {
                View = Views[0];
            }
            tab = Views.IndexOf(View!);
        }

        private Task SetCurrentUrl()
        {
            var url = Navigation.GetUriWithQueryParameter("View", Views[tab.ToInt32()]);
            NavigateService.SetCurrentUrl(url);
            return Task.CompletedTask;
        }

    }
}
