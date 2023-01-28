using BlazorComponent;
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
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Type { get; set; }
        private DateOnly _picker = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly Picker
        {
            get => _picker;
            set
            {
                if (_picker != value)
                {
                    _picker = value;
                    UpDiaries();
                }
            }
        }
        private Func<DateOnly, bool>? AllowedDates
        {
            get
            {
                var dateOnly = Diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct();
                return value => dateOnly.Contains(value);
            }
        }
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();
        private List<DiaryModel> CalendarDiaries => Diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == Picker).ToList();
        private StringNumber tabs = 0;
        private List<Tree> Trees => GetTrees();
        private List<int>? _active;
        private readonly List<string> Types = new()
        {
            "Calendar", "Tree"
        };
        private List<DiaryModel> TreeDiaries
        {
            get
            {
                if (_active == null || _active.Count == 0)
                {
                    return new();
                }
                var id = _active[0];
                return Diaries.Where(it => it.CreateTime.ToString("yyyyMMdd") == id.ToString()).ToList();
            }
        }
        private class Tree
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public List<Tree>? Children { get; set; }
        }
        protected override void OnInitialized()
        {
            SetTab();
            UpDiaries();
        }
        private async void UpDiaries()
        {
            Diaries = await DiaryService!.QueryAsync();
            StateHasChanged();
        }
        private void SetTab()
        {
            if (string.IsNullOrEmpty(Type))
            {
                Type = Types[0];
            }
            tabs = Types.IndexOf(Type!);
        }
        private async Task HandOnRefreshData(StringNumber value)
        {
            Diaries = await DiaryService!.QueryAsync();
            StateHasChanged();
            var url = Navigation!.GetUriWithQueryParameter("Type", Types[tabs.ToInt32()]);
            Navigation!.NavigateTo(url);
        }
        private List<Tree> GetTrees()
        {
            var years = Diaries.Select(it => it.CreateTime.Year).Distinct();
            List<Tree> yearTrees = new();
            foreach (var year in years)
            {
                var yearDiaries = Diaries.Where(it => it.CreateTime.Year == year);
                var months = yearDiaries.Select(it => it.CreateTime.Month).Distinct();
                List<Tree> monthTrees = new();
                foreach (var month in months)
                {
                    var monthDiaries = yearDiaries.Where(it => it.CreateTime.Month == month);
                    var days = monthDiaries.Select(it => it.CreateTime.Day).Distinct();
                    List<Tree> dayTrees = new();
                    foreach (var day in days)
                    {
                        var datetime = monthDiaries.Where(it => it.CreateTime.Day == day).First().CreateTime;
                        dayTrees.Add(new()
                        {
                            Id = Convert.ToInt32(datetime.ToString("yyyyMMdd")),
                            Name = day.ToString()
                        });
                    }
                    monthTrees.Add(new()
                    {
                        Id = Convert.ToInt32(year.ToString() + month.ToString()),
                        Name = month.ToString(),
                        Children = dayTrees
                    });
                }
                yearTrees.Add(new()
                {
                    Id = year,
                    Name = year.ToString(),
                    Children = monthTrees
                });
            }
            return yearTrees;
        }
        private void NavigateToSearch()
        {
            NavigateService!.NavigateTo("/Search");
        }
        private async Task OnActiveUpdate(List<Tree> trees)
        {
            Diaries = await DiaryService!.QueryAsync();
            StateHasChanged();
        }
    }
}
