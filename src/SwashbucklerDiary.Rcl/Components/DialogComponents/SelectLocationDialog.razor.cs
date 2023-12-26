using BlazorComponent.JSInterop;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectLocationDialog : DialogComponentBase
    {
        private bool _value;

        private bool showAdd;

        private List<LocationModel> _locations = [];

        private MCardText? mCardText;

        private MDialogExtension? myDialog;

        private string? internalLocation;

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public override bool Visible
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

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
            get => _locations.OrderByDescending(it => it.Name == internalLocation).ToList();
            set => _locations = value;
        }

        private async void SetValue(bool value)
        {
            if (_value != value)
            {
                if (value)
                {
                    Locations = await LocationService.QueryAsync();
                    internalLocation = Value;
                }
                _value = value;
                StateHasChanged();
            }
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
