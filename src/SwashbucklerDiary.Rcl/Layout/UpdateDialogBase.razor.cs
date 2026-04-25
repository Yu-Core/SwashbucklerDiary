using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract partial class UpdateDialogBase : DialogComponentBase
    {
        [Parameter]
        public Release? Value { get; set; }

        protected abstract Task ToUpdate();

        private async Task HandleDoNotShowAgain(MouseEventArgs args)
        {
            await InternalVisibleChanged(false);
            await SettingService.SetAsync(s => s.UpdatePrompt, false);
        }

        private async Task HandleUpdate(MouseEventArgs args)
        {
            await InternalVisibleChanged(false);
            await ToUpdate();
        }
    }
}
