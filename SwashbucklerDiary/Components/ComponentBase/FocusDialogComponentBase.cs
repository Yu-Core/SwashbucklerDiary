using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public abstract class FocusDialogComponentBase : DialogComponentBase
    {
        protected MTextField<string?> TextField = default!;
        protected MMDialog? myDialog;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                myDialog!.AfterShowContent = async _ => await FocusAsync();
            }
        }

        private async Task FocusAsync()
        {
            if (TextField != null && !TextField.IsFocused)
            {
                await TextField.InputElement.FocusAsync();
            }
        }
    }
}
