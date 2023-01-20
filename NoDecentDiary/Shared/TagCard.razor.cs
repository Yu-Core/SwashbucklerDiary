using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class TagCard : IDisposable
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }

        [Parameter]
        [EditorRequired]
        public TagModel? Value { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnRename { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }

        private bool _showMenu;
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
                    NavigateService!.Action += CloseMenu;
                }
                else
                {
                    NavigateService!.Action -= CloseMenu;
                }
            }
        }
        private void CloseMenu()
        {
            ShowMenu = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (ShowMenu)
            {
                NavigateService!.Action -= CloseMenu;
            }
            GC.SuppressFinalize(this);
        }
    }
}
