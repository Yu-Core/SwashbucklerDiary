using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyTabs
    {
        const string queryParameterKey = "tabs";

        private StringNumber? previousValue;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public StringNumber? Value { get; set; }

        [Parameter]
        public EventCallback<StringNumber?> ValueChanged { get; set; }

        [Parameter]
        public List<TabListItem> Items { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InitValueAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            UpdateQueryParameter();
        }

        private async Task InitValueAsync()
        {
            var uri = new Uri(NavigationManager.Uri);
            var queryString = QueryHelpers.ParseQuery(uri.Query);
            if (queryString.TryGetValue(queryParameterKey, out var queryParameterValue))
            {
                var index = Items.FindIndex(it => it.QueryParameterValue == queryParameterValue.ToString());
                if (index >= 0)
                {
                    Value = previousValue = index;
                    if (ValueChanged.HasDelegate)
                    {
                        await ValueChanged.InvokeAsync(Value);
                    }

                    return;
                }
            }

            Value = previousValue = 0;
        }

        private void UpdateQueryParameter()
        {
            if (previousValue != Value)
            {
                previousValue = Value;
                var index = Value?.ToInt32() ?? 0;
                var uri = NavigationManager.GetUriWithQueryParameter(queryParameterKey, Items[index].QueryParameterValue);
                NavigationManager.NavigateTo(uri, replace: true);
            }
        }
    }
}
