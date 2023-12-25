using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiMenu : DialogComponentBase, IDisposable
    {
        [Parameter]
        public override bool Visible
        {
            get => base.Visible;
            set => SetValue(value);
        }

        [Parameter]
        public RenderFragment? ButtonContent { get; set; }

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = new();

        public void Dispose()
        {
            if (Visible)
            {
                NavigateService.Action -= Close;
            }

            GC.SuppressFinalize(this);
        }

        protected void SetValue(bool value)
        {
            if (base.Visible == value)
            {
                return;
            }

            base.Visible = value;
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
            Visible = false;
            if (VisibleChanged.HasDelegate)
            {
                await VisibleChanged.InvokeAsync(false);
            }
        }

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }
    }
}
