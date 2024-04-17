using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectLocationDialog : ShowContentDialogComponentBase
    {
        private bool showAdd;

        private string? internalLocation;

        private List<LocationModel> locations = [];

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

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

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();

            internalLocation = Value;
        }

        private void SetSelectedLocation(LocationModel location)
        {
            var name = internalLocation == location.Name ? string.Empty : location.Name;
            internalLocation = name;
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
                await AlertService.Warning(I18n.T("Location.Repeat.Title"), I18n.T("Location.Repeat.Content"));
                return;
            }

            LocationModel location = new(name);
            var flag = await LocationService.AddAsync(location);
            if (!flag)
            {
                await AlertService.Error(I18n.T("Share.AddFail"));
                return;
            }

            locations.Insert(0, location);
            internalLocation = location.Name;
            StateHasChanged();
        }

        private async Task HandleOnOK()
        {
            await InternalVisibleChanged(false);
            Value = internalLocation;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(internalLocation);
            }
        }
    }
}
