using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomMNavigationDrawer : MNavigationDrawer
    {
        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Parameter]
        public bool? MyValue
        {
            get => base.Value;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<bool?> MyValueChanged
        {
            get => base.ValueChanged;
            set => base.ValueChanged = value;
        }

        [Parameter]
        public EventCallback OnAfterShowContent { get; set; }

        protected override ValueTask DisposeAsyncCore()
        {
            if (Value ?? false)
            {
                NavigateController.RemoveHistoryAction(Close);
            }

            return base.DisposeAsyncCore();
        }

        private void SetValue(bool? value)
        {
            if (base.Value == value)
            {
                return;
            }

            base.Value = value;
            if (value ?? false)
            {
                NavigateController.AddHistoryAction(Close);
                OnAfterShowContent.InvokeAsync();
            }
            else
            {
                NavigateController.RemoveHistoryAction(Close);
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
