using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class CardComponentBase<T> : MyComponentBase
    {
        protected Dictionary<string, object> activatorAttributes = [];

        [Parameter]
        public T Value { get; set; } = default!;

        [Parameter]
        public EventCallback<(T, Dictionary<string, object>)> OnMenu { get; set; }

        protected virtual void OpenMenu()
        {
            activatorAttributes = [];
            if (OnMenu.HasDelegate)
            {
                OnMenu.InvokeAsync((Value, activatorAttributes));
            }
        }
    }
}
