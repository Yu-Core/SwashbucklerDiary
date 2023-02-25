using SwashbucklerDiary.Components;
using SwashbucklerDiary.Config;

namespace SwashbucklerDiary.Pages
{
    public partial class LogPage : PageComponentBase
    {
        private List<string> LogTextList = new();

        protected override async Task OnInitializedAsync()
        {
            InitLogText();
            await HandleAchievements(Models.AchievementType.Log);
            await base.OnInitializedAsync();
        }

        private void InitLogText()
        {
            if (!File.Exists(SerilogConstants.filePath))
            {
                return;
            }
            using var stream = File.Open(SerilogConstants.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string? Line;
            while ((Line = reader.ReadLine()) != null)
            {
                LogTextList.Add(Line);
            }
        }
    }
}
