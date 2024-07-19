using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class InputDialog : FocusDialogComponentBase
    {
        private bool _value;

        private string? inputText;

        private bool showPassword;

        private MTextarea? mTextarea;

        [Parameter]
        public override bool Visible
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public EventCallback<string> OnOK { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public int MaxLength { get; set; } = 20;

        [Parameter]
        public string? OKText { get; set; }

        [Parameter]
        public bool Password { get; set; }

        [Parameter]
        public bool MultiLine { get; set; }

        private string PasswordIcon
            => Password ? (showPassword ? "mdi-eye" : "mdi-eye-off") : string.Empty;

        private string PasswordType
            => Password ? (showPassword ? "text" : "password") : string.Empty;

        protected override async Task FocusAsync()
        {
            if (!MultiLine)
            {
                await base.FocusAsync();
            }
            else
            {
                if (mTextarea is not null && !mTextarea.IsFocused)
                {
                    await Task.Delay(200);
                    await mTextarea.InputElement.FocusAsync();
                }
            }

        }

        private async Task HandleOnEnter()
        {
            if (!_value)
            {
                return;
            }

            await HandleOnOK();
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(inputText);
        }

        private void SetValue(bool value)
        {
            if (value == Visible)
            {
                return;
            }

            if (value)
            {
                inputText = Text;
            }

            _value = value;
        }
    }
}
