using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class InputTag
    {
        [Parameter]
        public bool Value { get; set; }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Text { get; set; }
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; } 

        protected virtual async Task HandleOnCancel(MouseEventArgs _)
        {
            await InternalValueChanged(false);
        }

        private async Task InternalValueChanged(bool value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }

            await ClearText();
        }

        private async Task HandleOnSave()
        {
            await OnSave.InvokeAsync();
            await ClearText();
        }

        private async Task ClearText()
        {
            Text = string.Empty;
            if (TextChanged.HasDelegate)
            {
                await TextChanged.InvokeAsync(Text);
            }
        }
    }
}
