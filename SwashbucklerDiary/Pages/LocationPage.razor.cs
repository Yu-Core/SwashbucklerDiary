using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class LocationPage : PageComponentBase
    {
        bool ShowRename;
        bool ShowDelete;
        bool ShowAdd;
        Action? OnDelete;
        LocationModel SelectLocation = new();
        List<LocationModel> Locations = new();

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateLocations();
            await base.OnInitializedAsync();
        }

        async Task UpdateLocations()
        {
            Locations = await LocationService.QueryAsync();
        }

        void RenameLocation(LocationModel location)
        {
            SelectLocation = location;
            StateHasChanged();
            ShowRename = true;
        }

        void DeleteLocation(LocationModel location)
        {
            OnDelete = null;
            OnDelete += async () =>
            {
                ShowDelete = false;
                bool flag = await LocationService.DeleteAsync(location);
                if (flag)
                {
                    Locations.Remove(location);
                    await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                    StateHasChanged();
                }
                else
                {
                    await AlertService.Error(I18n.T("Share.DeleteFail"));
                }
            };
            ShowDelete = true;
            StateHasChanged();
        }

        async Task SaveRename(string name)
        {
            ShowRename = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (Locations!.Any(it => it.Name == name))
            {
                await AlertService.Warning(I18n.T("Location.Repeat.Title"), I18n.T("Location.Repeat.Content"));
                return;
            }

            SelectLocation.Name = name;
            bool flag = await LocationService.UpdateAsync(SelectLocation);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
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
