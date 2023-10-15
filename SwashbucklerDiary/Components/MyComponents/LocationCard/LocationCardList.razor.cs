using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationCardList : CardListComponentBase<LocationModel>
    {
        bool ShowRename;

        bool ShowDelete;

        LocationModel SelectedLocation = new();

        [Inject]
        public ILocationService LocationService { get; set; } = default!;

        public async Task Delete(LocationModel location)
        {
            SelectedLocation = location;
            ShowDelete = true;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Rename(LocationModel location)
        {
            SelectedLocation = location;
            ShowRename = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private async Task ConfirmDelete()
        {
            var location = SelectedLocation;
            ShowDelete = false;
            bool flag = await LocationService.DeleteAsync(location);
            if (flag)
            {
                _value.Remove(location);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task ConfirmRename(string name)
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
            SelectedLocation.UpdateTime = DateTime.Now;
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

        private void LoadView()
        {
            SortOptions = new()
            {
                {"Sort.CreateTime.Desc", it => it.OrderByDescending(l => l.CreateTime) },
                {"Sort.CreateTime.Asc", it => it.OrderBy(l => l.CreateTime) },
            };
            SortItem = SortItems.First().Value;
        }
    }
}
