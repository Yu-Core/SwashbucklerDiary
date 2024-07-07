using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VisibleTextField : OpenCloseComponentBase, IDisposable
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

        public void Dispose()
        {
            if (Visible)
            {
                NavigateController.RemoveHistoryAction(CloseSearch);
            }

            GC.SuppressFinalize(this);
        }

        private async void CloseSearch()
        {
            await InternalVisibleChanged(false);
        }
    }
}
