using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Extend;

namespace SwashbucklerDiary.Components
{
    public partial class SponsorDialog : DialogComponentBase
    {
        private bool _value;
        private bool ShowThank;
        private StringNumber selection = 0;
        private string? CustomAmount;
        private string? ThankContent;
        private readonly static List<string> Amounts = new()
        {
            "5","20","99"
        };

        [Parameter]
        public override bool Value 
        { 
            get => _value; 
            set => SetValue(value); 
        }

        private bool ShowCustomAmount => selection == Amounts.Count;

        private void SetValue(bool value)
        {
            if(_value == value)
            {
                return;
            }

            if (value)
            {
                CustomAmount = string.Empty;
            }

            _value = value;
        }

        private async Task OnSponsor(MouseEventArgs mouseEventArgs)
        {
            await HandleOnCancel(mouseEventArgs);
            ThankContent = GetThankContent();
            ShowThank = true;
        }

        private string GetThankContent()
        {
            if (ShowCustomAmount)
            {
                var key = CustomAmount.MD5Encrytp32();
                var thankContent = I18n.T($"Easter egg.{key}", false);
                if (!string.IsNullOrEmpty(thankContent))
                {
                    return thankContent;
                }
            }

            return I18n.T("Amount.ThankContent");
        }
    }
}
