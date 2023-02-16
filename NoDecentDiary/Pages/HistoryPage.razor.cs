using BlazorComponent;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Pages
{
    public partial class HistoryPage : PageComponentBase
    {
        private DateOnly PickedDate = DateOnly.FromDateTime(DateTime.Now);
        private List<DiaryModel> Diaries = new();
        private StringNumber tabs = 0;
        private List<int>? _active;
        private readonly List<string> Types = new() { "Calendar", "Tree" };

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Type { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SetTab();
            await UpdateDiaries();
            await base.OnInitializedAsync();
        }

        private class Tree
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public List<Tree>? Children { get; set; }
        }

        private List<Tree> Trees => GetTrees();
        private List<DiaryModel> CalendarDiaries => Diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == PickedDate).ToList();
        private Func<DateOnly, bool> AllowedDates
        {
            get
            {
                var dateOnly = Diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct();
                return value => dateOnly.Contains(value);
            }
        }
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

        private async Task PickedDateChanged(DateOnly value)
        {
            if (PickedDate != value)
            {
                PickedDate = value;
                await UpdateDiaries();
            }
        }

        private async Task UpdateDiaries()
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

        private async Task RefreshData(StringNumber value)
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
            NavigateService.NavigateTo("/search");
        }
    }
}
