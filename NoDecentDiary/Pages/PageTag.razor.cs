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
    public partial class PageTag : IDisposable
    {
        [Inject]
        private ITagService? TagService { get; set; }
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        private NavigationManager? Navigation { get; set; }
        [Inject]
        private MasaBlazor? MasaBlazor { get; set; }
        [Parameter]
        public int Id { get; set; }
        private TagModel Tag = new TagModel();
        private List<DiaryModel> Diaries = new List<DiaryModel>();
        protected override async Task OnInitializedAsync()
        {
            var tagModel = await TagService!.FindAsync(Id);
            if (tagModel == null)
            {
                Navigation!.NavigateTo("/?Type=Tags");
                return;
            }
            Tag = tagModel;
            Diaries = await DiaryService!.GetDiariesByTagAsync(Id);
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }
        private void HandOnBack()
        {
            Navigation!.NavigateTo("/?Type=Tags");
        }
        private void HandOnToWrite()
        {
            Navigation!.NavigateTo($"/Write?TagId={Id}");
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }
    }
}
