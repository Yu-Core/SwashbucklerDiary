using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Components
{
    public partial class SponsorDialog : DialogComponentBase
    {
        private bool ShowCustomAmount => selection == Amounts.Count;
        private bool ShowThank;
        StringNumber selection = 0;
        private readonly static List<string> Amounts = new()
        {
            "5","20","99"
        };

        private async Task OnSponsor(MouseEventArgs mouseEventArgs)
        {
            await HandleOnCancel(mouseEventArgs);
            ShowThank = true;
        }
    }
}
