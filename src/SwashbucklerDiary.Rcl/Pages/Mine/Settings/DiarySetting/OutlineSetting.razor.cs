using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class OutlineSetting : ImportantComponentBase
    {
        private bool showOutline;

        private bool rightOutline;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showOutline = SettingService.Get(s => s.Outline);
            rightOutline = SettingService.Get(s => s.RigthOutline) != MasaBlazor.RTL;
        }

        private async Task HandleRigthOutlineOnChange((string key, bool value) args)
        {
            await SettingService.SetAsync(args.key, args.value != MasaBlazor.RTL);
        }
    }
}
