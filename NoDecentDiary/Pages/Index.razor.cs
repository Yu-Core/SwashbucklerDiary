using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class Index
    {
        [Inject]
        public I18n? I18n { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }
        private StringNumber tabs = 0;
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();

        protected override async Task OnInitializedAsync()
        {
            Diaries = (await DiaryService!.QueryAsync()).Take(50).OrderByDescending(it=>it.CreateTime).ToList();
        }
    }
}
