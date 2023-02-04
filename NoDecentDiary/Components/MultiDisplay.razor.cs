using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public partial class MultiDisplay : IDisposable
    {
        [Inject]
        public MasaBlazor? MasaBlazor { get; set; }

        [Parameter]
        public RenderFragment? MobileContent { get; set; }
        [Parameter]
        public RenderFragment? DesktopContent { get; set; }

        public void Dispose()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            return base.OnInitializedAsync();
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
