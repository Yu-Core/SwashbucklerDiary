using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Models;
using System.Diagnostics;

namespace NoDecentDiary.Pages
{
    public partial class PageWrite
    {
        [Inject]
        public MasaBlazor? MasaBlazor { get; set; }
        private readonly List<string> _weathers = new List<string>()
        {
            "晴","阴","小雨","中雨","大雨","小雪","中雪","大雪","雾",
        };
        private string? _weather;
        private bool showMenu;
        private bool showTitle;
        private bool showSelectTag;
        private bool IsDesktop => MasaBlazor!.Breakpoint.SmAndUp;
        private List<TagModel> SelectedTags = new List<TagModel>();

        protected override Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += () => { return InvokeAsync(this.StateHasChanged); };
            return base.OnInitializedAsync();
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
            showSelectTag = false;
        }
    }
}
