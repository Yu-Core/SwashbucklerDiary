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
        private List<LocationModel> Locations = new();
        private MCardText? mCardText;
        private MMDialog? myDialog;

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

        private async void SetValue(bool value)
        {
            if (_value != value)
            {
                if (value)
                {
                    Locations = await LocationService.QueryAsync();
                    Locations = Locations.OrderByDescending(it => it.Name == Location).ToList();
                }
                _value = value;
                StateHasChanged();
            }
        }

        async Task SetSelectedLocation(LocationModel location)
        {
            var name = Location == location.Name ? string.Empty : location.Name;
            Location = name;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(name);
            }

            await InternalValueChanged(false);
        }

        async Task SaveAdd(string name)
        {
            ShowAdd = false;
            await InternalValueChanged(false);
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
            Locations.Add(location);
            Location = location.Name;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(location.Name);
            }
            StateHasChanged();
        }

        private async Task ScrollToTop()
        {
            if (mCardText?.Ref != null)
            {
                await JS.ScrollTo(mCardText.Ref, 0);
            }
        }

        private async Task Reset()
        {
            Location = string.Empty;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(Location);
            }
        }
    }
}
