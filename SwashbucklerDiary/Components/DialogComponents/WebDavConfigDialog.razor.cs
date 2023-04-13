using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class WebDavConfigDialog : DialogFocusComponentBase
    {
        private bool _value;
        private MForm? MForm;
        private WebDavConfigForm configModel = new();
        private bool showPassword;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public WebDavConfigForm WebDavConfigForm { get; set; } = default!;
        [Parameter]
        public EventCallback<WebDavConfigForm> OnOK { get; set; }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(configModel);
        }

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    MForm?.ResetValidation();
                    configModel = WebDavConfigForm.DeepCopy();
                }
                _value = value;
            }
        }
    }
}
