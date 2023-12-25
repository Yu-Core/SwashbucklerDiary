using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocationSettingPage : ImportantComponentBase
    {
        bool showAdd;

        bool showSort;

        List<LocationModel> locations = [];

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateLocationsAsync();
            await base.OnInitializedAsync();
        }

        async Task UpdateLocationsAsync()
        {
            locations = await LocationService.QueryAsync();
        }

        private async Task SaveAdd(string name)
        {
            showAdd = false;
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

            await AlertService.Success(I18n.T("Share.AddSuccess"));
            locations.Add(location);
            StateHasChanged();
        }
    }
}
