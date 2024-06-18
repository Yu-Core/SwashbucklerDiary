using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class PMobileDatePickerExtension : PMobileDatePicker, IDisposable
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;

        [Parameter]
        public bool MyVisible
        {
            get => base.Visible;
            set => SetVisible(value);
        }

        [Parameter]
        public EventCallback<bool> MyVisibleChanged
        {
            get => base.VisibleChanged;
            set => base.VisibleChanged = value;
        }

        public void Dispose()
        {
            if (Visible)
            {
                NavigateService.RemoveHistoryAction(Close);
            }

            GC.SuppressFinalize(this);
        }

        private void SetVisible(bool value)
        {
            if (base.Visible == value)
            {
                return;
            }

            base.Visible = value;
            if (value)
            {
                NavigateService.AddHistoryAction(Close);
            }
            else
            {
                NavigateService.RemoveHistoryAction(Close);
            }
        }

        private async void Close()
        {
            MyVisible = false;
            if (MyVisibleChanged.HasDelegate)
            {
                await MyVisibleChanged.InvokeAsync(false);
            }
        }
    }
}
