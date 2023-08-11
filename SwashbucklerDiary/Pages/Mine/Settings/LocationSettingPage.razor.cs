using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class LocationSettingPage : PageComponentBase
    {
        bool ShowAdd;
        List<LocationModel> Locations = new();

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateLocationsAsync();
            await base.OnInitializedAsync();
        }

        async Task UpdateLocationsAsync()
        {
            Locations = await LocationService.QueryAsync();
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
            Locations.Add(location);
            StateHasChanged();
        }
    }
}
