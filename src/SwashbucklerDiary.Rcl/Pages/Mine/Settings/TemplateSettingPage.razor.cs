using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class TemplateSettingPage
    {
        private UseTemplateKind useTemplateMethod;

        private bool selectTemplateWhenCreate;

        private bool showUseTemplateMethod;

        private bool showDefaultTemplate;

        private List<TagModel> tags = [];

        private static readonly Dictionary<string, UseTemplateKind> useTemplateKindItems = new()
        {
            { "Cover", UseTemplateKind.Cover },
            { "Insert", UseTemplateKind.Insert },
        };

        [Inject]
        private ITagService TagService { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                tags = await TagService.QueryAsync();
                StateHasChanged();
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            useTemplateMethod = (UseTemplateKind)SettingService.Get(it => it.UseTemplateMethod);
            selectTemplateWhenCreate = SettingService.Get(it => it.SelectTemplateWhenCreate);
        }

        private async Task UseTemplateMethodChanged(UseTemplateKind value)
        {
            await SettingService.SetAsync(s => s.UseTemplateMethod, (int)value);
        }

        private async Task SetDefaultTemplateAsync(DiaryModel template)
        {
            showDefaultTemplate = false;
            await SettingService.SetAsync(it => it.DefaultTemplateId, template.Id.ToString());
        }
    }
}