using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool showTags;

        private bool showWeather;

        private bool showMood;

        private bool showLocation;

        private bool showTime;

        private bool showFileTypes;

        private bool showIconText;

        private string? search;

        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string scrollContainerSelector = string.Empty;

        private List<DynamicListItem> filterItems = [];

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
            LoadQuery();
            LoadView();
        }

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<List<TagModel>>(nameof(SelectedTags), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<StringNumber?>(nameof(SelectedWeather), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<StringNumber?>(nameof(SelectedMood), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<string?>(nameof(SelectedLocation), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<List<MediaResource>>(nameof(SelectedFileTypes), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<DateFilterForm>(nameof(DateFilterForm), UpdateDiariesAndStateHasChangedAsync);
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showIconText = SettingService.Get(it => it.DiaryIconText);
        }

        protected override async Task UpdateDiariesAsync()
        {
            Expression<Func<DiaryModel, bool>> exp = GetExpression();
            var diaries = await DiaryService.QueryAsync(exp);
            var tagNames = SelectedTags.Select(it => it.Name).ToList();
            Diaries = diaries.WhereIF(IsTagsFiltered, it => it.Tags != null && tagNames.All(tagName => it.Tags.Any(tag => tag.Name == tagName)))
                .WhereIF(IsFileTypeFiltered, it => it.Resources != null && SelectedFileTypes.All(fileType => it.Resources.Any(r => r.ResourceType == fileType)))
                .OrderByDescending(it => it.CreateTime)
                .ToList();
        }

        private DateOnly MinDate => DateFilterForm.GetDateMinValue();

        private DateOnly MaxDate => DateFilterForm.GetDateMaxValue();

        private bool IsTagsFiltered => SelectedTags.Count > 0;

        private bool IsWeatherFiltered => SelectedWeather is not null;

        private bool IsMoodFiltered => SelectedMood is not null;

        private bool IsLocationFiltered => !string.IsNullOrEmpty(SelectedLocation);

        private bool IsFileTypeFiltered => SelectedFileTypes.Count > 0;

        private bool IsTimeFiltered => MinDate != default || MaxDate != default;

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private bool ShowClearFilter => IsTagsFiltered || IsWeatherFiltered || IsMoodFiltered || IsLocationFiltered || IsFileTypeFiltered || IsTimeFiltered;

        private List<TagModel> SelectedTags
        {
            get => GetValue<List<TagModel>>() ?? [];
            set => SetValue(value);
        }

        private StringNumber? SelectedWeather
        {
            get => GetValue<StringNumber?>();
            set => SetValue(value);
        }

        private StringNumber? SelectedMood
        {
            get => GetValue<StringNumber?>();
            set => SetValue(value);
        }

        private string? SelectedLocation
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        private List<MediaResource> SelectedFileTypes
        {
            get => GetValue<List<MediaResource>>([]) ?? [];
            set => SetValue(value);
        }

        private DateFilterForm DateFilterForm
        {
            get => GetValue<DateFilterForm>(new())!;
            set => SetValue(value);
        }

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                search = Query;
            }
        }

        private void LoadView()
        {
            filterItems =
            [
                new(this,"Search.Filter.Tags",()=>showTags=true,()=>IsTagsFiltered),
                new(this,"Search.Filter.Weather",()=>showWeather=true,()=>IsWeatherFiltered),
                new(this,"Search.Filter.Mood",()=>showMood=true,()=>IsMoodFiltered),
                new(this,"Search.Filter.Location",()=>showLocation=true,()=>IsLocationFiltered),
                new(this,"Search.Filter.File",()=>showFileTypes=true,()=>IsFileTypeFiltered),
                new(this,"Search.Filter.Time",()=>showTime=true,()=>IsTimeFiltered),
            ];
        }

        private async Task UpdateDiariesAndStateHasChangedAsync()
        {
            await UpdateDiariesAsync();
            StateHasChanged();
        }

        private async Task ClearFilter()
        {
            SetValueWithNoEffect<List<TagModel>>([], nameof(SelectedTags));
            SetValueWithNoEffect<StringNumber>(null, nameof(SelectedWeather));
            SetValueWithNoEffect<StringNumber>(null, nameof(SelectedMood));
            SetValueWithNoEffect<string>(null, nameof(SelectedLocation));
            SetValueWithNoEffect<List<MediaResource>>([], nameof(SelectedFileTypes));
            SetValueWithNoEffect<DateFilterForm>(new(), nameof(DateFilterForm));
            await UpdateDiariesAsync();
        }

        private Expression<Func<DiaryModel, bool>> GetExpression()
        {
            Expression<Func<DiaryModel, bool>>? exp = null;

            if (IsTagsFiltered)
            {
                exp = exp.And(it => true);
            }

            if (IsWeatherFiltered)
            {
                var selectedWeather = SelectedWeather?.ToString();
                Expression<Func<DiaryModel, bool>> expWeather
                    = it => it.Weather == selectedWeather;
                exp = exp.And(expWeather);
            }

            if (IsMoodFiltered)
            {
                var selectedMood = SelectedMood?.ToString();
                Expression<Func<DiaryModel, bool>> expMood
                    = it => it.Mood == selectedMood;
                exp = exp.And(expMood);
            }

            if (IsLocationFiltered)
            {
                Expression<Func<DiaryModel, bool>> expLocation
                    = it => it.Location == SelectedLocation;
                exp = exp.And(expLocation);
            }

            if (IsFileTypeFiltered)
            {
                exp = exp.And(it => true);
            }

            if (MinDate != default)
            {
                DateTime DateTimeMin = MinDate.ToDateTime(default);
                Expression<Func<DiaryModel, bool>> expMinDate = it => it.CreateTime >= DateTimeMin;
                exp = exp.And(expMinDate);
            }

            if (MaxDate != default)
            {
                DateTime DateTimeMax = MaxDate.ToDateTime(TimeOnly.MaxValue);
                Expression<Func<DiaryModel, bool>> expMaxDate = it => it.CreateTime <= DateTimeMax;
                exp = exp.And(expMaxDate);
            }

            if (IsSearchFiltered)
            {
                Expression<Func<DiaryModel, bool>> expSearch
                    = it => (it.Title ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                    || (it.Content ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => false;
            }

            return exp;
        }

        private void ToRead(DiaryModel diary)
        {
            string? queryParameters = string.IsNullOrWhiteSpace(search) ? null : $"?query={search}";
            To($"read/{diary.Id}{queryParameters}");
        }
    }
}
