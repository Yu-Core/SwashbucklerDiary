using BlazorComponent.JSInterop;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class SelectLocation : DialogComponentBase
    {
        private bool _value;

        private bool ShowAdd;

        private List<LocationModel> _locations = new();

        private MCardText? mCardText;

        private MDialogExtension? myDialog;

        private string? InternalLocation;

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public string? Location { get; set; }

        [Parameter]
        public EventCallback<string> LocationChanged { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                myDialog!.AfterShowContent = async _ => { await ScrollToTop(); };
            }

        }

        private List<LocationModel> Locations
        {
            get => _locations.OrderByDescending(it => it.Name == InternalLocation).ToList();
            set => _locations = value;
        }

        private async void SetValue(bool value)
        {
            if (_value != value)
            {
                if (value)
                {
                    Locations = await LocationService.QueryAsync();
                    InternalLocation = Location;
                }
                _value = value;
                StateHasChanged();
            }
        }

        private async Task SetSelectedLocation(LocationModel location)
        {
            var name = InternalLocation == location.Name ? string.Empty : location.Name;
            InternalLocation = name;
            await ScrollToTop();
        }

        private async Task SaveAdd(string name)
        {
            ShowAdd = false;
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
            InternalLocation = location.Name;
            await InvokeAsync(StateHasChanged);
            await ScrollToTop();
        }

        private async Task HandleOnOK()
        {
            await InternalValueChanged(false);
            Location = InternalLocation;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(InternalLocation);
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
