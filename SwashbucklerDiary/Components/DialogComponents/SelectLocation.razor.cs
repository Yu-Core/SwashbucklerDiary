using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.IServices;
using Microsoft.Maui.Devices.Sensors;

namespace SwashbucklerDiary.Components
{
    public partial class SelectLocation : DialogComponentBase
    {
        private bool _value;
        private bool ShowAdd;
        private List<LocationModel> Locations = new();

        [Inject]
        ILocationService LocationService { get; set; } = default!;

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
            Location = location.Name;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(location.Name);
            }
            await InternalValueChanged(false);
        }

        async Task SaveAdd(string name)
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
            Locations.Add(location);
            Location = location.Name;
            if (LocationChanged.HasDelegate)
            {
                await LocationChanged.InvokeAsync(location.Name);
            }
            StateHasChanged();
        }
    }
}
