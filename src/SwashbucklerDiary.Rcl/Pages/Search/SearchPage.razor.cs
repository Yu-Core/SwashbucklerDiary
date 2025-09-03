using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SqlSugar;
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

        private bool showFilterCondition;

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
                   .Watch<List<string>>(nameof(SelectedWeathers), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<List<string>>(nameof(SelectedMoods), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<List<string>>(nameof(SelectedLocations), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<List<MediaResource>>(nameof(SelectedFileTypes), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<DateFilterForm>(nameof(DateFilterForm), UpdateDiariesAndStateHasChangedAsync)
                   .Watch<SearchFilterConditionForm>(nameof(SearchFilterConditionForm), UpdateDiariesAndStateHasChangedAsync);
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showIconText = SettingService.Get(it => it.DiaryIconText);
        }

        protected override async Task UpdateDiariesAsync()
        {
            if (!(IsTagsFiltered || IsWeatherFiltered || IsMoodFiltered || IsLocationFiltered || IsFileTypeFiltered || IsTimeFiltered || IsSearchFiltered))
            {
                Diaries = [];
                return;
            }

            Expression<Func<DiaryModel, bool>> exp = CreateExpression();
            var diaries = await DiaryService.QueryDiariesAsync(exp);

            Diaries = diaries.OrderByDescending(it => it.CreateTime)
                .ToList();
        }

        private DateOnly MinDate => DateFilterForm.GetDateMinValue();

        private DateOnly MaxDate => DateFilterForm.GetDateMaxValue();

        private bool IsTagsFiltered => SelectedTags.Count > 0;

        private bool IsWeatherFiltered => SelectedWeathers.Count > 0;

        private bool IsMoodFiltered => SelectedMoods.Count > 0;

        private bool IsLocationFiltered => SelectedLocations.Count > 0;

        private bool IsFileTypeFiltered => SelectedFileTypes.Count > 0;

        private bool IsTimeFiltered => MinDate != default || MaxDate != default;

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private bool ShowClearFilter => IsTagsFiltered || IsWeatherFiltered || IsMoodFiltered || IsLocationFiltered || IsFileTypeFiltered || IsTimeFiltered;

        private List<TagModel> SelectedTags
        {
            get => GetValue<List<TagModel>>([])!;
            set => SetValue(value);
        }

        private List<string> SelectedWeathers
        {
            get => GetValue<List<string>>([])!;
            set => SetValue(value);
        }

        private List<string> SelectedMoods
        {
            get => GetValue<List<string>>([])!;
            set => SetValue(value);
        }

        private List<string> SelectedLocations
        {
            get => GetValue<List<string>>([])!;
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


        private SearchFilterConditionForm SearchFilterConditionForm
        {
            get => GetValue<SearchFilterConditionForm>(new())!;
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
                new(this,"Tag",()=>showTags=true,()=>IsTagsFiltered),
                new(this,"Weather",()=>showWeather=true,()=>IsWeatherFiltered),
                new(this,"Mood",()=>showMood=true,()=>IsMoodFiltered),
                new(this,"Location",()=>showLocation=true,()=>IsLocationFiltered),
                new(this,"File",()=>showFileTypes=true,()=>IsFileTypeFiltered),
                new(this,"Time",()=>showTime=true,()=>IsTimeFiltered),
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
            SetValueWithNoEffect<List<string>>([], nameof(SelectedWeathers));
            SetValueWithNoEffect<List<string>>([], nameof(SelectedMoods));
            SetValueWithNoEffect<List<string>>([], nameof(SelectedLocations));
            SetValueWithNoEffect<List<MediaResource>>([], nameof(SelectedFileTypes));
            SetValueWithNoEffect<DateFilterForm>(new(), nameof(DateFilterForm));
            await UpdateDiariesAsync();
        }

        private Expression<Func<DiaryModel, bool>> CreateExpression()
        {
            var expable = Expressionable.Create<DiaryModel>();

            if (IsTagsFiltered)
            {
                var selectedTagIds = SelectedTags.Select(t => t.Id).ToList();
                Expression<Func<DiaryModel, bool>>? expTags = SearchFilterConditionForm.Tags == FilterCondition.AllComply
                    ? it => it.Id == SqlFunc.Subqueryable<DiaryTagModel>()
                        .Where(dt => selectedTagIds.Contains(dt.TagId))
                        .GroupBy(dt => dt.DiaryId)
                        .Having(dt => SqlFunc.AggregateCount(dt.DiaryId) == selectedTagIds.Count)
                        .Select(dt => dt.DiaryId)
                    : it => it.Tags.Any(tag => selectedTagIds.Contains(tag.Id));

                expable.And(expTags);
            }

            if (IsWeatherFiltered)
            {
                Expression<Func<DiaryModel, bool>> expWeather = SearchFilterConditionForm.Weathers == FilterCondition.AllComply
                    ? SelectedWeathers.Count > 1
                        ? it => false
                        : it => it.Weather == SelectedWeathers.First()
                    : it => SelectedWeathers.Contains(it.Weather);
                expable.And(expWeather);
            }

            if (IsMoodFiltered)
            {
                Expression<Func<DiaryModel, bool>> expMood = SearchFilterConditionForm.Moods == FilterCondition.AllComply
                    ? SelectedMoods.Count > 1
                        ? it => false
                        : it => it.Mood == SelectedMoods.First()
                    : it => SelectedMoods.Contains(it.Mood);
                expable.And(expMood);
            }

            if (IsLocationFiltered)
            {
                Expression<Func<DiaryModel, bool>> expLocation = SearchFilterConditionForm.Locations == FilterCondition.AllComply
                    ? SelectedLocations.Count > 1
                        ? it => false
                        : it => it.Location == SelectedLocations.First()
                    : it => SelectedLocations.Contains(it.Location);
                expable.And(expLocation);
            }

            if (IsFileTypeFiltered)
            {
                Expression<Func<DiaryModel, bool>> expFiles = SearchFilterConditionForm.FileTypes == FilterCondition.AllComply
                    ? it => it.Id == SqlFunc.Subqueryable<DiaryResourceModel>()
                        .LeftJoin<ResourceModel>((dr, r) => dr.ResourceUri == r.ResourceUri)
                        .Where((dr, r) => SelectedFileTypes.Contains(r.ResourceType))
                        .GroupBy((dr, r) => dr.DiaryId)
                        .Having(dr => SqlFunc.MappingColumn<bool>($"COUNT(DISTINCT `r`.`ResourceType`) = {SelectedFileTypes.Count}"))
                        .Select(dr => dr.DiaryId)
                    : it => it.Resources.Any(r => SelectedFileTypes.Contains(r.ResourceType));

                expable.And(expFiles);
            }

            if (MinDate != default)
            {
                DateTime dateTimeMin = MinDate.ToDateTime(default);
                Expression<Func<DiaryModel, bool>> expMinDate = it => it.CreateTime >= dateTimeMin;
                expable.And(expMinDate);
            }

            if (MaxDate != default)
            {
                DateTime dateTimeMax = MaxDate.ToDateTime(TimeOnly.MaxValue);
                Expression<Func<DiaryModel, bool>> expMaxDate = it => it.CreateTime <= dateTimeMax;
                expable.And(expMaxDate);
            }

            if (IsSearchFiltered)
            {
                Expression<Func<DiaryModel, bool>> expSearch
                    = it => (it.Title ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                    || (it.Content ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                expable.And(expSearch);
            }

            return expable.ToExpression();
        }

        private void ToRead(DiaryModel diary)
        {
            string? queryParameters = string.IsNullOrWhiteSpace(search) ? null : $"?query={search}";
            To($"read/{diary.Id}{queryParameters}");
        }

        private void SaveSearchFilterCondition(SearchFilterConditionForm form)
        {
            showFilterCondition = false;
            SearchFilterConditionForm = form;
        }
    }
}
