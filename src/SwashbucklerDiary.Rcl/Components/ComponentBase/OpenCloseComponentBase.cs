using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class OpenCloseComponentBase : MyComponentBase
    {
        [Parameter]
        public virtual bool Visible { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        protected virtual async Task InternalVisibleChanged(bool value)
        {
            Visible = value;

            if (VisibleChanged.HasDelegate)
            {
                await VisibleChanged.InvokeAsync(value);
            }
        }
    }
}
