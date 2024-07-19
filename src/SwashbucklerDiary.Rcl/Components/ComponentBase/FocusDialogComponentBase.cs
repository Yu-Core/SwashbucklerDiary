using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class FocusDialogComponentBase : DialogComponentBase
    {
        protected MTextField<string?> textField = default!;

        protected virtual async Task FocusAsync()
        {
            if (textField is not null && !textField.IsFocused)
            {
                await Task.Delay(200);
                await textField.InputElement.FocusAsync();
            }
        }
    }
}
