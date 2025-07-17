using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomMBottomSheet : MBottomSheet
    {
        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

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

        [Parameter]
        public EventCallback<bool> OnAfterShowContent { get; set; }

        [Parameter]
        public EventCallback OnBeforeShowContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AfterShowContent = HandleOnAfterShowContent;
            BeforeShowContent = HandleOnBeforeShowContent;
        }

        protected override ValueTask DisposeAsyncCore()
        {
            if (Value)
            {
                NavigateController.RemoveHistoryAction(Close);
            }

            return base.DisposeAsyncCore();
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
                NavigateController.AddHistoryAction(Close);
            }
            else
            {
                NavigateController.RemoveHistoryAction(Close);
            }
        }

        private void Close()
        {
            InvokeAsync(async () =>
            {
                MyValue = false;
                if (MyValueChanged.HasDelegate)
                {
                    await MyValueChanged.InvokeAsync(false);
                }
            });
        }

        private async Task HandleOnAfterShowContent(bool value)
        {
            if (OnAfterShowContent.HasDelegate)
            {
                await OnAfterShowContent.InvokeAsync(value);
            }
        }

        private async Task HandleOnBeforeShowContent()
        {
            if (OnBeforeShowContent.HasDelegate)
            {
                await OnBeforeShowContent.InvokeAsync();
            }
        }
    }
}
