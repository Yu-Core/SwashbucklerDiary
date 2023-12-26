using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchAgreement
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback OnOK { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private string ShowClass => Show ? "" : "d-none";
    }
}
