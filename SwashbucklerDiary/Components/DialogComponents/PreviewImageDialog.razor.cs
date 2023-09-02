using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        [Parameter]
        public string? Src { get; set; }

        private string FilePath
        {
            get
            {
                string src = StaticCustomScheme.ReverseCustomSchemeRender(Src);
                return AppDataService.CustomSchemeUriToFilePath(src);
            }
        }

        private async Task SaveToLocal()
        {
            var name = Path.GetFileName(FilePath);
            var path = await PlatformService.SaveFileAsync(name, FilePath);
            if(path is not null)
            {
                await AlertService.Success(I18n.T("Share.SaveSuccess"));
            }
        }

        private async Task Share()
        {
            await PlatformService.ShareFile(I18n.T("Share.Share"), FilePath);
            await HandleAchievements(AchievementType.Share);
        }
    }
}
