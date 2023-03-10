using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Components
{
    public partial class InputDialog : DialogFocusComponentBase
    {
        private bool _value;
        private string? InputText;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Text { get; set; }
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }
        [Parameter]
        public EventCallback<string> OnOK { get; set; }
        [Parameter]
        public string? Placeholder { get; set; }
        [Parameter]
        public int MaxLength { get; set; } = 20;

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    InputText = Text;
                }
                _value = value;
            }
        }

        protected virtual async Task HandleOnEnter(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await HandleOnOK();
            }
        }

        protected async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(InputText);
        }
    }
}
