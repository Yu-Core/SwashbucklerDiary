using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VisibleTextField : OpenCloseComponentBase
    {
        protected bool _visible;

        [Parameter]
        public override bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnInput { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (Visible)
            {
                NavigateController.RemoveHistoryAction(CloseSearch);
            }
        }

        protected async Task InternalValueChanged(string? value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        private async void SetVisible(bool value)
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;

            if (value)
            {
                NavigateController.AddHistoryAction(CloseSearch);
            }
            else
            {
                NavigateController.RemoveHistoryAction(CloseSearch);
                await InternalValueChanged(string.Empty);
                if (OnInput.HasDelegate)
                {
                    await OnInput.InvokeAsync(string.Empty);
                }
            }
        }

        private async void CloseSearch()
        {
            await InternalVisibleChanged(false);
        }
    }
}
