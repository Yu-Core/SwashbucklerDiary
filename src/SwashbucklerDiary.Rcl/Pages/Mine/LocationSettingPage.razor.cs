using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocationSettingPage : ImportantComponentBase
    {
        bool showAdd;

        List<LocationModel> locations = [];

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateLocationsAsync();
                StateHasChanged();
            }
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
                await PopupServiceHelper.Warning(I18n.T("Location already exists"), I18n.T("Do not add again"));
                return;
            }

            LocationModel location = new(name);
            var flag = await LocationService.AddAsync(location);
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Add failed"));
                return;
            }

            locations.Insert(0, location);
            locations = [.. locations];
            StateHasChanged();
        }
    }
}
