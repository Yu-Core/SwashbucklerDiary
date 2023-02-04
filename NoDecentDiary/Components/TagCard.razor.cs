using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Components
{
    public partial class TagCard : MyComponentBase, IDisposable
    {
        private bool _showMenu;

        [Parameter]
        public TagModel? Value { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnRename { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }

        public void Dispose()
        {
            if (ShowMenu)
            {
                NavigateService.Action -= CloseMenu;
            }
            GC.SuppressFinalize(this);
        }

        private bool ShowMenu
        {
            get => _showMenu;
            set
            {
                SetShowMenu(value);
            }
        }

        private void SetShowMenu(bool value)
        {
            if (_showMenu != value)
            {
                _showMenu = value;
                if (value)
                {
                    NavigateService.Action += CloseMenu;
                }
                else
                {
                    NavigateService.Action -= CloseMenu;
                }
            }
        }

        private void CloseMenu()
        {
            ShowMenu = false;
            StateHasChanged();
        }
    }
}
