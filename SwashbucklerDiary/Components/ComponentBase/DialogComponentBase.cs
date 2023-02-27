using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Components
{
    public abstract class DialogComponentBase : MyComponentBase
    {
        [Parameter]
        public virtual bool Value { get; set; }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }

        protected virtual async Task HandleOnCancel(MouseEventArgs _)
        {
            await InternalValueChanged(false);
        }

        protected virtual async Task InternalValueChanged(bool value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }
    }
}
