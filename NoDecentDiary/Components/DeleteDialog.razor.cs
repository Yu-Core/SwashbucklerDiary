using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public partial class DeleteDialog : MyComponentBase, IDisposable
    {
        private bool _value;

        [Parameter]
        public bool Value
        {
            get => _value;
            set
            {
                SetValue(value);
            }
        }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Content { get; set; }
        [Parameter]
        public EventCallback OnOK { get; set; }

        public void Dispose()
        {
            if (Value)
            {
                NavigateService.Action -= Close;
            }
            GC.SuppressFinalize(this);
        }

        protected virtual async Task OnCancel(MouseEventArgs _)
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

        private void SetValue(bool value)
        {
            if (_value != value)
            {
                _value = value;
                if (value)
                {
                    NavigateService.Action += Close;
                }
                else
                {
                    NavigateService.Action -= Close;
                }
            }
        }

        private async void Close()
        {
            await InternalValueChanged(false);
            StateHasChanged();
        }
        
    }
}
