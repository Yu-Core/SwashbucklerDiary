using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MultiSearchControl
    {
        private string? SearchContent;

        [Parameter]
        public Func<string?,Task>? OnSearch { get; set; }

        private async Task Search()
        {
            if(OnSearch is null)
            {
                return;
            }

            await OnSearch.Invoke(SearchContent);
        }
    }
}
