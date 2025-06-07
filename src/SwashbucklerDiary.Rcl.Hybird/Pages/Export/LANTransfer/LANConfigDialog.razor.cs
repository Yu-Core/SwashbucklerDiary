using DeepCloner.Core;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANConfigDialog : FocusDialogComponentBase
    {
        private MForm? MForm;

        private LANConfigForm configModel = new();

        [Parameter]
        public LANConfigForm Form { get; set; } = default!;

        [Parameter]
        public EventCallback<LANConfigForm> OnOK { get; set; }

        [Parameter]
        public EventCallback OnReset { get; set; }

        private void BeforeShowContent()
        {
            MForm?.Reset();
            configModel = Form.DeepClone();
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(configModel);
        }
    }
}
