using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MySearch : DialogComponentBase,IDisposable
    {
        private bool _value;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public string? Search { get; set; }

        [Parameter]
        public EventCallback<string> SearchChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnChanged { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        private bool ShowTitle => !string.IsNullOrEmpty(Title);

        private string? TextFieldColor => Light ? "grey" : null;

        private void SetValue(bool value)
        {
            if (_value != value)
            {
                _value = value;
                if (!ShowTitle)
                {
                    return;
                }

                Task.Run(() =>
                {
                    if (value)
                    {
                        NavigateService.Action += CloseSearch;
                    }
                    else
                    {
                        Search = string.Empty;
                        NavigateService.Action -= CloseSearch;
                    }
                });
            }
        }

        public void Dispose()
        {
            NavigateService.Action -= CloseSearch;
            GC.SuppressFinalize(this);
        }

        private async void CloseSearch()
        {
            await InternalValueChanged(false);
        }

        private async Task InternalSearchChanged(string? value)
        {
            Search = value;
            if (SearchChanged.HasDelegate)
            {
                await SearchChanged.InvokeAsync(Search);
            }
            if (OnChanged.HasDelegate)
            {
                await OnChanged.InvokeAsync(Search);
            }
        }
    }
}
