using Microsoft.AspNetCore.Components;

namespace NoDecentDiary.Components
{
    public partial class InputTag : DialogComponentBase
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
        public EventCallback OnSave { get; set; }

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

        private async Task HandleOnSave()
        {
            Text = InputText;
            if (TextChanged.HasDelegate)
            {
                await TextChanged.InvokeAsync(Text);
            }
            await OnSave.InvokeAsync();
        }

    }
}
