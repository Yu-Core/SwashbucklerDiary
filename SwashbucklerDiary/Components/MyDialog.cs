using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public class MyDialog : MDialog
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

        public new async ValueTask DisposeAsync()
        {
            if (Value)
            {
                NavigateService.Action -= Close;
            }
            await base.DisposeAsync();
        }

        private void SetValue(bool value)
        {
            if (base.Value != value)
            {
                base.Value = value;
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
