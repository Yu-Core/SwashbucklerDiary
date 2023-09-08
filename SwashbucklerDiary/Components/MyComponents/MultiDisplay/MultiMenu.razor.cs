using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;
using Util.Reflection.Expressions;

namespace SwashbucklerDiary.Components
{
    public partial class MultiMenu : DialogComponentBase, IDisposable
    {
        [Parameter]
        public override bool Value
        {
            get => base.Value;
            set => SetValue(value);
        }
        [Parameter]
        public RenderFragment? ButtonContent { get; set; }
        [Parameter]
        public List<DynamicListItem> ListItemModels { get; set; } = new();

        private async Task OnClick(EventCallback callback)
        {
            await InternalValueChanged(false);
            await callback.InvokeAsync();
        }

        public void Dispose()
        {
            if (Value)
            {
                NavigateService.Action -= Close;
            }

            GC.SuppressFinalize(this);
        }

        protected void SetValue(bool value)
        {
            if (base.Value == value)
            {
                return;
            }

            base.Value = value;
            if (value)
            {
                NavigateService.Action += Close;
            }
            else
            {
                NavigateService.Action -= Close;
            }
        }

        private async void Close()
        {
            Value = false;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(false);
            }
        }

        private async Task UpdateDisplay(bool value)
        {
            if (Value)
            {
                await InternalValueChanged(false);
            }
        }
    }
}
