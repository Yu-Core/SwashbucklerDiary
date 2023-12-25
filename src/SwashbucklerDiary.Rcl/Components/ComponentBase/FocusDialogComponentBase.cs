using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class FocusDialogComponentBase : DialogComponentBase
    {
        protected MTextField<string?> textField = default!;

        protected MDialogExtension mDialogExtension = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if(mDialogExtension is null)
                {
                    return;
                }

                mDialogExtension.AfterShowContent = async _ => await DialogAfterRenderAsync();
            }
        }

        protected virtual async Task DialogAfterRenderAsync()
        {
            await FocusAsync();
        }

        private async Task FocusAsync()
        {
            if (textField != null && !textField.IsFocused)
            {
                await textField.InputElement.FocusAsync();
            }
        }
    }
}
