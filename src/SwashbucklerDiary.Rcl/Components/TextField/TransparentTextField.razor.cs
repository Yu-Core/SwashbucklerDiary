using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TransparentTextField
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnInput { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public bool Clearable { get; set; } = true;

        [Parameter]
        public bool Autofocus { get; set; } = true;

        private string? TextFieldColor => Dark ? "white" : "grey";
    }
}