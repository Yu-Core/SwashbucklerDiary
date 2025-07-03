using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class WelcomeAgreement
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

        private string Class => new CssBuilder()
            .Add("px-4")
            .Add("d-none", !Show)
            .ToString();
    }
}
