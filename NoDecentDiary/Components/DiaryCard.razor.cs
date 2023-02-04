using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Reflection.Expressions.IntelligentGeneration.Extensions;

namespace NoDecentDiary.Components
{
    public partial class DiaryCard : MyComponentBase, IDisposable
    {
        private bool _showMenu;

        [Parameter]
        public DiaryModel? Value { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public EventCallback OnTopping { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnCopy { get; set; }
        [Parameter]
        public EventCallback OnTag { get; set; }
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

        private DateTime Date => Value!.CreateTime;
        private string? Title => Value!.Title;
        private string? Text => Value!.Content;
        private bool IsTop => Value!.Top;
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
