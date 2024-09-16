using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectLocationDialog : DialogComponentBase
    {
        private bool showAdd;

        private bool showSearch;

        private string? searchText;

        private string? selectedLocation;

        private List<LocationModel> locations = [];

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                locations = await LocationService.QueryAsync();
                StateHasChanged();
            }
        }

        private string? Color => Dark ? "white" : "grey";

        private ICollection<LocationModel> FilterLocations =>
            string.IsNullOrWhiteSpace(searchText) ? locations : locations.Where(it => !string.IsNullOrEmpty(it.Name) && (it.Name.Contains(searchText) || it.Name == selectedLocation)).ToList();

        private void BeforeShowContent()
        {
            selectedLocation = Value;
        }

        private void SetSelectedLocation(LocationModel location)
        {
            var name = selectedLocation == location.Name ? string.Empty : location.Name;
            selectedLocation = name;
        }

        private async Task SaveAdd(string name)
        {
            showAdd = false;
            StateHasChanged();

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (locations.Any(it => it.Name == name))
            {
                await PopupServiceHelper.Warning(I18n.T("Location.Repeat.Title"), I18n.T("Location.Repeat.Content"));
                return;
            }

            LocationModel location = new(name);
            var flag = await LocationService.AddAsync(location);
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Share.AddFail"));
                return;
            }

            locations.Insert(0, location);
            selectedLocation = location.Name;
            StateHasChanged();
        }

        private async Task HandleOnOK()
        {
            await InternalVisibleChanged(false);
            Value = selectedLocation;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(selectedLocation);
            }
        }
    }
}
