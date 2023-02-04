using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Services;
using System.Diagnostics;

namespace NoDecentDiary.Pages
{
    public partial class WritePage : IDisposable
    {
        [Inject]
        public MasaBlazor? MasaBlazor { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        public IconService? IconService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? TagId { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public int? DiaryId { get; set; }
        private bool showTitle;
        private bool _showMenu;
        private bool ShowMenu
        {
            get => _showMenu;
            set => SetShowMenu(value);
        }
        private bool _showSelectTag;
        private bool ShowSelectTag
        {
            get => _showSelectTag;
            set => SetShowSelectTag(value);
        }
        private bool _showWeather;
        private bool ShowWeather
        {
            get => _showWeather;
            set => SetShowWeather(value);
        }
        private bool _showMood;
        private bool ShowMood
        {
            get => _showMood;
            set => SetShowMood(value);
        }
        private bool _showLocation;
        private bool ShowLocation
        {
            get => _showLocation;
            set => SetShowLocation(value);
        }
        private DiaryModel Diary = new DiaryModel()
        {
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        private bool Desktop => MasaBlazor!.Breakpoint.SmAndUp;
        private bool Mobile => !MasaBlazor!.Breakpoint.SmAndUp;
        private List<TagModel> SelectedTags = new List<TagModel>();
        private Dictionary<string, string> WeatherIcons => IconService!.WeatherIcon;
        private Dictionary<string, string> MoodIcons => IconService!.MoodIcon;
        private StringNumber WeatherIndex
        {
            get
            {
                if (string.IsNullOrEmpty(Diary.Weather))
                {
                    return -1;
                }
                return WeatherIcons.Keys.ToList().IndexOf(Diary.Weather);
            }
            set
            {
                if (value != null)
                {
                    Diary.Weather = WeatherIcons.ElementAt(value.ToInt32()).Key;
                }
                else
                {
                    Diary.Weather = string.Empty;
                }
            }
        }
        private StringNumber MoodIndex
        {
            get
            {
                if (string.IsNullOrEmpty(Diary.Mood))
                {
                    return -1;
                }
                return MoodIcons.Keys.ToList().IndexOf(Diary.Mood);
            }
            set
            {
                if (value != null)
                {
                    Diary.Mood = MoodIcons.ElementAt(value.ToInt32()).Key;
                }
                else
                {
                    Diary.Mood = string.Empty;
                }
            }
        }
        private string Weather =>
            string.IsNullOrEmpty(Diary.Weather) ? I18n!.T("Write.Weather") : I18n!.T("Weather." + Diary.Weather);
        private string Mood =>
            string.IsNullOrEmpty(Diary.Mood) ? I18n!.T("Write.Mood") : I18n!.T("Mood." + Diary.Mood);
        
        protected override async Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            NavigateService!.Action += HandOnBack;
            await SetTag();
            await SetDiary();
        }

        private async Task SetTag()
        {
            if (TagId != null)
            {
                var tag = await TagService!.FindAsync((int)TagId);
                if (tag != null)
                {
                    SelectedTags.Add(tag);
                }
            }
        }
        private async Task SetDiary()
        {
            if (DiaryId != null)
            {
                var diary = await DiaryService!.FindAsync((int)DiaryId);
                if (diary != null)
                {
                    Diary = diary;
                    showTitle = !string.IsNullOrEmpty(diary.Title);
                    SelectedTags = await TagService!.GetDiaryTagsAsync((int)DiaryId);
                }
            }
        }
        private void RemoveSelectedTag(TagModel tag)
        {
            int index = SelectedTags.IndexOf(tag);
            if (index > -1)
            {
                SelectedTags.RemoveAt(index);
            }
        }
        private void HandOnSaveSelectTags()
        {
            ShowSelectTag = false;
        }
        private async Task HandOnSave()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }
            await SaveDiary();
        }
        private async void HandOnBack()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                NavigateToBack();
                return;
            }

            await SaveDiary();
        }
        private void HandOnClear()
        {
            Diary.Content = string.Empty;
            this.StateHasChanged();
        }
        private async Task SaveDiary()
        {
            if (DiaryId == null)
            {
                bool flag = await DiaryService!.AddAsync(Diary);
                if (flag)
                {
                    int id = await DiaryService.GetLastInsertRowId();
                    await DiaryTagService!.AddTagsAsync(id, SelectedTags);
                    await PopupService!.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n!.T("Share.AddSuccess");
                    });
                }
                else
                {
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Error;
                        it.Title = I18n!.T("Share.AddFail");
                    });
                }
            }
            else
            {
                bool flag = await DiaryService!.UpdateAsync(Diary);
                if (flag)
                {
                    await DiaryTagService!.DeleteAsync(it => it.DiaryId == DiaryId);
                    await DiaryTagService!.AddTagsAsync((int)DiaryId, SelectedTags);
                    await PopupService!.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n!.T("Share.EditSuccess");
                    });

                }
                else
                {
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Error;
                        it.Title = I18n!.T("Share.EditFail");
                    });
                }
            }

            NavigateToBack();
        }
        public void NavigateToBack()
        {
            NavigateService!.NavigateToBack();
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
        private string GetWeatherIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-weather-cloudy";
            }
            return IconService!.GetWeatherIcon(key);
        }
        private string GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-emoticon-outline";
            }
            return IconService!.GetMoodIcon(key);
        }
        private string CounterValue(string? value)
        {
            int len = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return len + " " + I18n!.T("Write.CountUnit");
            }

            value = value.Trim();
            if(I18n!.T("Write.Word") == "1")
            {
                len = value.Split(' ').Length;
            }

            if(I18n!.T("Write.Character") == "1")
            {
                len = value.Length;
            }

            return len + " " + I18n!.T("Write.CountUnit");
        }
        private void SetShowMenu(bool value)
        {
            if (_showMenu != value)
            {
                _showMenu = value;
                if (value)
                {
                    NavigateService!.Action += CloseMenu;
                }
                else
                {
                    NavigateService!.Action -= CloseMenu;
                }
            }
        }
        private void CloseMenu()
        {
            ShowMenu = false;
            StateHasChanged();
        }
        private void SetShowSelectTag(bool value)
        {
            if (_showSelectTag != value)
            {
                _showSelectTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseSelectTag;
                }
                else
                {
                    NavigateService!.Action -= CloseSelectTag;
                }
            }
        }
        private void CloseSelectTag()
        {
            ShowSelectTag = false;
            StateHasChanged();
        }
        private void SetShowWeather(bool value)
        {
            if (_showWeather != value)
            {
                _showWeather = value;
                if (value)
                {
                    NavigateService!.Action += CloseWeather;
                }
                else
                {
                    NavigateService!.Action -= CloseWeather;
                }
            }
        }
        private void CloseWeather()
        {
            ShowWeather = false;
            StateHasChanged();
        }
        private void SetShowMood(bool value)
        {
            if (_showMood != value)
            {
                _showMood = value;
                if (value)
                {
                    NavigateService!.Action += CloseMood;
                }
                else
                {
                    NavigateService!.Action -= CloseMood;
                }
            }
        }
        private void CloseMood()
        {
            ShowMood = false;
            StateHasChanged();
        }
        private void SetShowLocation(bool value)
        {
            if (_showLocation != value)
            {
                _showLocation = value;
                if (value)
                {
                    NavigateService!.Action += CloseLocation;
                }
                else
                {
                    NavigateService!.Action -= CloseLocation;
                }
            }
        }
        private void CloseLocation()
        {
            ShowLocation = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            if (ShowMenu)
            {
                NavigateService!.Action -= CloseMenu;
            }
            if (ShowSelectTag)
            {
                NavigateService!.Action -= CloseSelectTag;
            }
            NavigateService!.Action -= HandOnBack;
            GC.SuppressFinalize(this);
        }
    }
}
