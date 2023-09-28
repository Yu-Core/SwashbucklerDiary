using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Shared
{
    public partial class FirstLaunchLanguage
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public bool Show { get; set; }
        [Parameter]
        public EventCallback<string> OnClick { get; set; }

        private string ShowClass => Show ? "" : "d-none";

        private Task HandleClick(string value) => OnClick.InvokeAsync(value);
        
    }
}
