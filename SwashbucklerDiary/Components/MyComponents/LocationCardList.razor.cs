using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationCardList : MyComponentBase
    {
        bool ShowRename;
        bool ShowDelete;
        LocationModel SelectedLocation = new();

        [Inject]
        public ILocationService LocationService { get; set; } = default!;

        [Parameter]
        public List<LocationModel> Value { get; set; } = new();

        private void OpenDeleteDialog(LocationModel location)
        {
            SelectedLocation = location;
            ShowDelete = true;
        }

        private async Task HandleDelete(LocationModel location)
        {
            ShowDelete = false;
            bool flag = await LocationService.DeleteAsync(location);
            if (flag)
            {
                Value.Remove(location);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private void OpenRenameDialog(LocationModel location)
        {
            SelectedLocation = location;
            StateHasChanged();
            ShowRename = true;
        }

        private async Task HandleRename(string name)
        {
            ShowRename = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (Value.Any(it => it.Name == name))
            {
                await AlertService.Warning(I18n.T("Location.Repeat.Title"), I18n.T("Location.Repeat.Content"));
                return;
            }

            SelectedLocation.Name = name;
            bool flag = await LocationService.UpdateAsync(SelectedLocation);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }
    }
}
