using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageMine
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        
    }
}
