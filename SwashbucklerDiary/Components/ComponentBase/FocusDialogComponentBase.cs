using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public abstract class FocusDialogComponentBase : DialogComponentBase
    {
        protected MTextField<string?> TextField = default!;

        protected MDialogExtension? myDialog;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if(myDialog is null)
                {
                    return;
                }

                myDialog.AfterShowContent = async _ => await DialogAfterRenderAsync();
            }
        }

        protected virtual async Task DialogAfterRenderAsync()
        {
            await FocusAsync();
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
