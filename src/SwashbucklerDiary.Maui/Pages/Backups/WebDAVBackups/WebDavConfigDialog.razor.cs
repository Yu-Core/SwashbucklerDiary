using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Pages
{
    public partial class WebDavConfigDialog : FocusDialogComponentBase
    {
        private MForm? mForm;

        private WebDavConfigForm configModel = new();

        private bool showPassword;

        [Parameter]
        public WebDavConfigForm Form { get; set; } = default!;

        [Parameter]
        public EventCallback<WebDavConfigForm> OnOK { get; set; }

        private void BeforeShowContent()
        {
            mForm?.Reset();
            configModel = Form.DeepCopy();
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(configModel);
        }


    }
}
