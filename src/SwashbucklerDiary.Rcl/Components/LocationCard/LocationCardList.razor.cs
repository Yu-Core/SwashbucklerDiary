using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LocationCardList : CardListComponentBase<LocationModel>
    {
        bool showRename;

        bool showDelete;

        [Inject]
        public ILocationService LocationService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            var locationSort = SettingService.Get(s => s.LocationSort);
            if (!string.IsNullOrEmpty(locationSort))
            {
                SortItem = locationSort;
            }
        }

        private async Task ConfirmDelete()
        {
            showDelete = false;
            bool flag = await LocationService.DeleteAsync(SelectedItem);
            if (flag)
            {
                if (RemoveSelectedItem())
                {
                    await AlertService.Success(I18n.T("Delete successfully"));
                    StateHasChanged();
                }
            }
            else
            {
                await AlertService.Error(I18n.T("Delete failed"));
            }
        }

        private async Task ConfirmRename(string name)
        {
            showRename = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (Value.Any(it => it.Name == name))
            {
                await AlertService.Warning(I18n.T("Location already exists"), I18n.T("Do not add again"));
                return;
            }

            SelectedItem.Name = name;
            SelectedItem.UpdateTime = DateTime.Now;
            bool flag = await LocationService.UpdateAsync(SelectedItem, it => new { it.Name, it.UpdateTime });
            if (!flag)
            {
                await AlertService.Error(I18n.T("Change failed"));
            }
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Time - Reverse order", it => it.OrderByDescending(l => l.CreateTime) },
                {"Time - Positive order", it => it.OrderBy(l => l.CreateTime) },
            };

            if (string.IsNullOrEmpty(SortItem))
            {
                SortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Rename", "edit_square", Rename),
                new(this, "Delete", "mdi:mdi-delete-outline", Delete),
                new(this, "Sort", "sort", OpenSortDialog),
            ];
        }

        private void Delete()
        {
            showDelete = true;
        }

        private void Rename()
        {
            showRename = true;
        }

        private async Task SortChanged(string value)
        {
            await SettingService.SetAsync(s => s.LocationSort, value);
        }
    }
}
