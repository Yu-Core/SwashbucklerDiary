using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NoDecentDiary.Components
{
    public partial class MyMarkdown
    {
        private MMarkdown? mMarkdown;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }
        [Parameter]
        public string? Class { get; set; }

        private Dictionary<string, object> _options = new()
        {
            {"mode","ir" },
            {"counter",
                new
                {
                    enable = true,
                    type = "type"
                }
            },
            {"minHeight",240},
            {"toolbar",
                new List<string>()
                {
                    "headings", "bold", "italic", "strike", "line", "quote",
                    "list", "ordered-list" , "check", "indent","code","inline-code",
                    "link","emoji","edit-mode"
                } 
            }
        };

//        private async Task OnFocus(string value)
//        {
//#if ! __MOBILE__
//            return;
//#endif
//            await JS.InvokeVoidAsync("raiseToolBar",new object[1] { mMarkdown!.Ref });
//        }

//        private async Task OnBlur(string value)
//        {
//#if !__MOBILE__
//            return;
//#endif
//            await JS.InvokeVoidAsync("reduceToolBar", new object[1] { mMarkdown!.Ref });
//        }
    }
}
