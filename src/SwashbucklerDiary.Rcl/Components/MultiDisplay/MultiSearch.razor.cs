using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSearch
    {
        private string? searchText;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public EventCallback<string> OnSearch { get; set; }

        private string? TextFieldColor => Dark ? "white" : "grey";

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
