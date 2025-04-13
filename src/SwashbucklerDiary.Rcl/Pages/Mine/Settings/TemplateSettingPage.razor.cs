using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class TemplateSettingPage
    {
        private UseTemplateKind useTemplateMethod;

        private bool selectTemplateWhenCreate;

        private bool showUseTemplateMethod;

        private static readonly Dictionary<string, UseTemplateKind> useTemplateKindItems = new()
        {
            { "Cover", UseTemplateKind.Cover },
            { "Insert", UseTemplateKind.Insert },
        };

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
    }
}