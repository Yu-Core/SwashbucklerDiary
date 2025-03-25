using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchAgreement
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback OnOK { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private string ShowClass => Show ? "px-4" : "d-none";
    }
}
