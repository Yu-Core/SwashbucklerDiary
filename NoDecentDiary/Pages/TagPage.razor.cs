using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Components;
using NoDecentDiary.Extend;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class TagPage : PageComponentBase, IDisposable
    {
        private TagModel Tag = new();
        private List<DiaryModel> Diaries = new();

        [Inject]
        private ITagService TagService { get; set; } = default!;
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            var tagModel = await TagService!.FindAsync(Id);
            if (tagModel == null)
            {
                NavigateToBack();
                return;
            }
            Tag = tagModel;
            Diaries = await DiaryService!.GetDiariesByTagAsync(Id);
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }

        private bool Prominent => MasaBlazor.Breakpoint.SmAndUp && Diaries.Any();

        private void NavigateToWrite()
        {
            NavigateService.NavigateTo($"/write?tagId={Id}");
        }

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
