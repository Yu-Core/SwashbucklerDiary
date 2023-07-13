using CommunityToolkit.Maui.Storage;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<string?> PickFolderAsync()
        {
            try
            {
                var folder = await FolderPicker.Default.PickAsync(default);
                if (folder.IsSuccessful)
                {
                    return folder.Folder.Path;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
