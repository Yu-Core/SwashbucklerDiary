using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NoDecentDiary.Components
{
    public partial class SponsorDialog : DialogComponentBase, IDisposable
    {
        private bool ShowCustomAmount;
        private bool ShowThank;
        private readonly static List<string> Amounts = new()
        {
            "5","20","99"
        };

        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            return base.OnInitializedAsync();
        }

        private bool IsDesktop => MasaBlazor.Breakpoint.SmAndUp;

        private async Task OnSponsor(MouseEventArgs mouseEventArgs)
        {
            await HandleOnCancel(mouseEventArgs);
            ShowThank = true;
        }


        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
