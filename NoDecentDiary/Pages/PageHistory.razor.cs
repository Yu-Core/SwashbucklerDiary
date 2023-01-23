using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageHistory
    {
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        private DateOnly _picker = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly Picker
        {
            get => _picker;
            set
            {
                _picker= value;
                UpdateDiaries();
            }
        }
        private readonly Func<DateOnly, bool> AllowedDates = value => value <= DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] ArrayEvents = Array.Empty<DateOnly>();
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();
        protected override void OnInitialized()
        {
            UpdateDiaries();
        }
        private async void UpdateDiaries()
        {
            var diaries = await DiaryService!.QueryAsync();
            Diaries = diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == Picker).ToList();
            ArrayEvents = diaries.Select(it => DateOnly.FromDateTime(it.CreateTime))
                .Distinct()
                .ToArray();
            StateHasChanged();
        }
    }
}
