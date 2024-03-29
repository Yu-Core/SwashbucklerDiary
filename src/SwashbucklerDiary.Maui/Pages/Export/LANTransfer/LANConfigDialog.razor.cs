﻿using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Pages
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

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();

            MForm?.Reset();
            configModel = Form.DeepCopy();
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(configModel);
        }
    }
}
