using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SettingSwitch
    {
        [Inject]
        private ISettingService SettingService { get; set; } = default!;

        [Parameter]
        public bool Value { get; set; }

        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }

        [EditorRequired]
        [Parameter]
        public string SettingKey { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<(string, bool)> OnChange { get; set; }

        private async Task HandleOnChange(bool value)
        {
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync((SettingKey, value));
            }
            else
            {
                await SettingService.SetAsync(SettingKey, value);
            }
        }
    }
}