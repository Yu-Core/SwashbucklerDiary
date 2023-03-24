using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
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

        private string GetIconColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted && Light ? "black" : "";
        }

        private static string GetIcon(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted ? "mdi-trophy" : "mdi-trophy-outline";
        }

        private static int GetProgressRate(AchievementModel achievement)
        {
            double percent = (double)achievement.UserAchievement.CompleteRate / achievement.Steps * 100;
            return (int)percent;
        }

        private string GetProgressRateColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted && Light ? "#2e2e2e" : "grey lighten-1";
        }

        private string GetProgressRateTextColor(AchievementModel achievement)
        {
            return achievement.UserAchievement.IsCompleted && Light ? "white--text" : "";
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
