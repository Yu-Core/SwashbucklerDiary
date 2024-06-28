using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SearchTextField : OpenCloseComponentBase, IDisposable
    {
        private bool _visible;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public override bool Visible
        {
            get => _visible;
            set => SetValue(value);
        }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnInput { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        protected bool Light => !Dark;

        private bool ShowTitle => !string.IsNullOrEmpty(Title);

        private string? TextFieldColor => Light ? "grey" : null;

        private void SetValue(bool value)
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;
            if (!ShowTitle)
            {
                return;
            }

            if (value)
            {
                NavigateController.AddHistoryAction(CloseSearch);
            }
            else
            {
                Value = string.Empty;
                NavigateController.RemoveHistoryAction(CloseSearch);
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

        private async Task InternalValueChanged(string? value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }
    }
}
