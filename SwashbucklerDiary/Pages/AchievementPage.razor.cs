using BlazorComponent.I18n;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class AchievementPage : PageComponentBase
    {
        private List<AchievementModel> Achievements = new();

        protected override async Task OnInitializedAsync()
        {
            await SetAchievements();
            await base.OnInitializedAsync();
        }

        private async Task SetAchievements()
        {
            var achievements = await AchievementService.GetAchievements();
            Achievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private static string GetIconColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted ? "black" : "";
        }

        private static string GetIcon(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted ? "mdi-star-check" : "mdi-star-outline";
        }

        private static int GetProgressRate(AchievementModel achievement)
        {
            double percent = (double)achievement.UserAchievement.CompleteRate / achievement.Steps * 100;
            return (int)percent;
        }

        private static string GetProgressRateColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted ? "grey darken-4" : "grey lighten-1";
        }

        private static string GetProgressRateTextColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted ? "white--text" : "";
        }

        private string GetProgressRateText(AchievementModel achievement)
        {
            if (achievement.UserAchievement.IsCompleted)
            {
                return $"{I18n.T("Achievement.Completed")} {achievement.UserAchievement.CompletedTime:d}";
            }
            else
            {
                return $"{achievement.UserAchievement.CompleteRate} / {achievement.Steps}";
            }
        }
    }
}
