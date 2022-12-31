using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class AddTag
    {
        [Parameter]
        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
        [Parameter]
        public string? Text { get; set; }
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; } 

        private bool value;
        private string? Content { get; set; }

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
        }

        private async Task HandleOnSave()
        {
            Text = Content;
            
            if (TextChanged.HasDelegate)
            {
                await TextChanged.InvokeAsync(Content);
            }
            await OnSave.InvokeAsync();
            Content = string.Empty;
        }
    }
}
