using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public class MyBottomSheet : MBottomSheet
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;

        [Parameter]
        public bool MyValue
        {
            get => base.Value;
            set
            {
                base.Value = value;
                SetValue(value);
            }
        }
        [Parameter]
        public EventCallback<bool> MyValueChanged
        {
            get => base.ValueChanged;
            set => base.ValueChanged = value;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (Value)
            {
                NavigateService.Action -= Close;
            }
        }

        private void SetValue(bool value)
        {
            Task.Run(() =>
            {
                if (value)
                {
                    NavigateService.Action += Close;
                }
                else
                {
                    NavigateService.Action -= Close;
                }
            });
        }

        private async void Close()
        {
            Value = false;
            if (MyValueChanged.HasDelegate)
            {
                await MyValueChanged.InvokeAsync(false);
            }
        }
    }
}
