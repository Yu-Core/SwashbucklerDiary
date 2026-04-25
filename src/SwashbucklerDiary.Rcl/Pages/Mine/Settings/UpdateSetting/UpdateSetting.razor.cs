using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UpdateSetting : ImportantComponentBase
    {
        private bool updatePrompt;
        private int updatePromptInterval;
        private UpdateMethod updateMethod;
        private bool showUpdatePromptIntervalDialog;
        private bool showUpdateMethodDialog;

        private readonly Dictionary<string, int> updatePromptIntervalItems = new()
        {
            { "Daily", 1 },
            { "Weekly", 7 },
            { "Monthly", 30 },
        };
        private readonly Dictionary<string, UpdateMethod> updateMethodItems = new()
        {
            { "Ask every time", UpdateMethod.AskEveryTime },
            { "App store", UpdateMethod.AppStore },
            { "GitHub releases", UpdateMethod.GithubReleases },
            { "China Mobile Cloud Disk", UpdateMethod.ChinaMobileCloudDisk },
        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            updatePrompt = SettingService.Get(s => s.UpdatePrompt);
            updatePromptInterval = SettingService.Get(s => s.UpdatePromptIntervalDay);
            updateMethod = (UpdateMethod)SettingService.Get(s => s.UpdateMethod);
        }

        private string UpdatePromptFrequencyText => I18n.T(updatePromptIntervalItems.FirstOrDefault(i => i.Value == updatePromptInterval).Key ?? string.Empty);
        private string UpdateMethodText => I18n.T(updateMethodItems.FirstOrDefault(i => i.Value == updateMethod).Key ?? string.Empty);

        private async Task UpdateMethodChanged(UpdateMethod value)
        {
            await SettingService.SetAsync(s => s.UpdateMethod, (int)value);
        }

        private async Task UpdatePromptIntervalChanged(int value)
        {
            await SettingService.SetAsync(s => s.UpdatePromptIntervalDay, value);
        }
    }
}