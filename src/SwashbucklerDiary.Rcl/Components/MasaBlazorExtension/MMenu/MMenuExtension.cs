using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MMenuExtension : MMenu
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;

        [Parameter]
        public bool MyValue
        {
            get => base.Value;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<bool> MyValueChanged
        {
            get => base.ValueChanged;
            set => base.ValueChanged = value;
        }

        protected override ValueTask DisposeAsync(bool disposing)
        {
            if (Value)
            {
                NavigateService.Action -= Close;
            }

            return base.DisposeAsync(disposing);
        }

        private void SetValue(bool value)
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
            MyValue = false;
            if (MyValueChanged.HasDelegate)
            {
                await MyValueChanged.InvokeAsync(false);
            }
        }
    }
}
