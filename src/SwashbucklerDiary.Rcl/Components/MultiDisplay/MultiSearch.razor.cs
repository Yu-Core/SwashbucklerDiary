using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSearch
    {
        private string? searchText;

        [Parameter]
        public EventCallback<string> OnSearch { get; set; }

        private async Task Search()
        {
            if (OnSearch.HasDelegate)
            {
                await OnSearch.InvokeAsync(searchText);
            }

            searchText = string.Empty;
        }
    }
}
