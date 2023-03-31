using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private DateOnly PickedDate = DateOnly.FromDateTime(DateTime.Now);
        private StringNumber tab = 0;
        private List<int>? _active;
        private List<Tree> Trees = new();
        private Func<DateOnly, bool> AllowedDates = PickedDate => true;
        private readonly List<string> Types = new() { "Calendar", "Tree" };

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Type { get; set; }

        protected override async Task OnInitializedAsync()
        {
            InitTab();
            SetCurrentUrl();
            await base.OnInitializedAsync();
            UpdateTrees();
            UpdateAllowDates();
        }

        private class Tree
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public List<Tree>? Children { get; set; }
        }

        private List<DiaryModel> CalendarDiaries => Diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == PickedDate).ToList();
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

        private void InitTab()
        {
            if (string.IsNullOrEmpty(Type))
            {
                Type = Types[0];
            }
            tab = Types.IndexOf(Type!);
        }

        private void SetCurrentUrl()
        {
            NavigateService.CurrentUrl += () => {
                return Navigation.GetUriWithQueryParameter("Type", Types[tab.ToInt32()]);
            };
        }

        private void UpdateTrees()
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
            Trees = yearTrees;
        }

        private async Task Update()
        {
            await UpdateDiaries();
            UpdateTrees();
            UpdateAllowDates();
        }

        private void UpdateAllowDates()
        {
            var dateOnly = Diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct();
            AllowedDates = value => dateOnly.Contains(value);
        }
    }
}
