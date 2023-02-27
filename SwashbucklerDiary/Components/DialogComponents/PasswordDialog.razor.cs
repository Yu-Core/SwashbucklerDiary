using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Components
{
    public partial class PasswordDialog : DialogComponentBase
    {
        private bool _value;
        private Model model = new();
        private MForm? form;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }
        [Parameter]
        public EventCallback<string> OnOK { get; set; }
        [Parameter]
        public string? Placeholder { get; set; }
        [Parameter]
        public int MaxLength { get; set; } = 20;

        class Model
        {
            [Required]
            [MaxLength(10, ErrorMessage = "Password must be at most 20 characters long")]
            [MinLength(10, ErrorMessage = "Password must be at least 6 characters long")]
            public string? Password { get; set; }
            [Required]
            [Compare("Password",ErrorMessage = "The two passwords are inconsistent")]
            public string? PasswordConfirmation { get; set; }
        }

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    form?.Reset();
                }
                _value = value;
            }
        }

        private async Task HandleOnOK()
        {
            await OnOK.InvokeAsync(model.Password);
        }
    }
}
