using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public abstract class MyComponentBase : ComponentBase
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;
        [Inject]
        protected I18n I18n { get; set; } = default!;
        [Inject]
        protected IPopupService PopupService { get; set; } = default!;

        protected NavigationManager Navigation => NavigateService.Navigation!;

        protected Task ToDo()
        {
            return PopupService.ToastAsync(it =>
            {
                it.Type = AlertTypes.Info;
                it.Title = I18n!.T("ToDo.Title");
                it.Content = I18n!.T("ToDo.Content");
            });
        }
    }
}
