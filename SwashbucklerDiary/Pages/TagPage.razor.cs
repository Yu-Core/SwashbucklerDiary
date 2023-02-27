using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class TagPage : PageComponentBase, IDisposable
    {
        private TagModel Tag = new();

        [Inject]
        private ITagService TagService { get; set; } = default!;
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
            var tagModel = await TagService.FindIncludesAsync(Id);
            if (tagModel == null)
            {
                NavigateToBack();
                return;
            }
            Tag = tagModel;
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }


        private List<DiaryModel> Diaries => Tag.Diaries ?? new();

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
