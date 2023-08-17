using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Extend;

namespace SwashbucklerDiary.Pages
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

        protected override async Task DialogAfterRenderAsync()
        {
            MForm?.Reset();
            configModel = Form.DeepCopy();
            await base.DialogAfterRenderAsync();
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(configModel);
        }
    }
}
