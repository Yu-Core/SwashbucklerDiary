using BlazorComponent.JSInterop;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectLocationDialog : ShowContentDialogComponentBase
    {
        private bool showAdd;

        private List<LocationModel> _locations = [];

        private MCardText mCardText = default!;

        private string? internalLocation;

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        protected override async Task AfterShowContent(bool isLazyContent)
        {
            await base.AfterShowContent(isLazyContent);
            await ScrollToTop();
        }

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();
            await SetLocations();
        }

        private List<LocationModel> Locations
        {
            get => _locations.OrderByDescending(it => it.Name == internalLocation).ToList();
            set => _locations = value;
        }

        private async Task SetLocations()
        {
            Locations = await LocationService.QueryAsync();
            internalLocation = Value;
        }

        private async Task SetSelectedLocation(LocationModel location)
        {
            var name = internalLocation == location.Name ? string.Empty : location.Name;
            internalLocation = name;
            await ScrollToTop();
        }

        private async Task SaveAdd(string name)
        {
            showAdd = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (Locations.Any(it => it.Name == name))
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

            await AlertService.Success(I18n.T("Share.AddSuccess"));
            var locations = Locations;
            locations.Add(location);
            Locations = locations;
            internalLocation = location.Name;
            await InvokeAsync(StateHasChanged);
            await ScrollToTop();
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

        private async Task ScrollToTop()
        {
            if (mCardText?.Ref != null)
            {
                await JS.ScrollTo(mCardText.Ref, 0);
            }
        }
    }
}
