using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace NoDecentDiary.Components
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
        public EventCallback OnOK { get; set; }
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

        private async Task HandleOnOK()
        {
            Text = InputText;
            if (TextChanged.HasDelegate)
            {
                await TextChanged.InvokeAsync(Text);
            }
            await OnOK.InvokeAsync();
        }

    }
}
