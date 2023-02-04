using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageAbout
    {
        [Inject]
        private INavigateService? NavigateService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }

        private void HandOnBack()
        {
            NavigateService!.NavigateToBack();
        }
    }
}
