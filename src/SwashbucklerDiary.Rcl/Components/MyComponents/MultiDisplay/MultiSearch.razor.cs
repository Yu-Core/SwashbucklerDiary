using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSearch : ImportantComponentBase
    {
        private string? SearchContent;

        [Parameter]
        public Func<string?, Task>? OnSearch { get; set; }

        protected override async Task OnResume()
        {
            SearchContent = string.Empty;
            await base.OnResume();
        }

        private async Task Search()
        {
            if (OnSearch is null)
            {
                return;
            }

            await OnSearch.Invoke(SearchContent);
        }

        protected async Task HandleOnEnter(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await Search();
            }
        }
    }
}
