using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public abstract class DialogFocusComponentBase : DialogComponentBase
    {
        private bool FirstOpen = true;
        protected MTextField<string?> TextField = default!;
        protected override void OnAfterRender(bool firstRender)
        {
            if (Value)
            {
                Task.Run(async () =>
                {
                    if(FirstOpen)
                    {
                        await Task.Delay(800);
                        FirstOpen = false;
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                    
                    if (TextField != null && !TextField.IsFocused)
                    {
                        await TextField.InputElement.FocusAsync();
                    }
                });
            }
            base.OnAfterRender(firstRender);
        }
    }
}
