using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LocationCardList : CardListComponentBase<LocationModel>
    {
        bool ShowRename;

        bool ShowDelete;

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

            var locationSort = SettingService.Get<string>(Setting.LocationSort);
            if (!string.IsNullOrEmpty(locationSort))
            {
                sortItem = locationSort;
            }
        }

        private async Task ConfirmDelete()
        {
            var location = SelectedItemValue;
            ShowDelete = false;
            bool flag = await LocationService.DeleteAsync(location);
            if (flag)
            {
                _value.Remove(location);
                await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await PopupServiceHelper.Error(I18n.T("Share.DeleteFail"));
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
                await PopupServiceHelper.Warning(I18n.T("Location.Repeat.Title"), I18n.T("Location.Repeat.Content"));
                return;
            }

            SelectedItemValue.Name = name;
            SelectedItemValue.UpdateTime = DateTime.Now;
            bool flag = await LocationService.UpdateAsync(SelectedItemValue, it => new { it.Name, it.UpdateTime });
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Share.EditFail"));
            }
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Sort.Time.Desc", it => it.OrderByDescending(l => l.CreateTime) },
                {"Sort.Time.Asc", it => it.OrderBy(l => l.CreateTime) },
            };

            if (string.IsNullOrEmpty(sortItem))
            {
                sortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Share.Rename", "mdi-rename-outline", Rename),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, "Share.Sort", "mdi-sort-variant", Sort),
            ];
        }

        private void Delete()
        {
            ShowDelete = true;
        }

        private void Rename()
        {
            ShowRename = true;
        }

        private async Task SortChanged(string value)
        {
            await SettingService.Set(Setting.LocationSort, value);
        }
    }
}
