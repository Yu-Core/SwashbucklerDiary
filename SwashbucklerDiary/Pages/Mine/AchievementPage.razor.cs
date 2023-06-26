using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class AchievementPage : PageComponentBase
    {
        private string? Search;
        private bool ShowSearch;
        private List<AchievementModel> AllAchievements = new();
        private List<AchievementModel> Achievements = new();

        protected override async Task OnInitializedAsync()
        {
            await SetAchievements();
            await base.OnInitializedAsync();
        }

        protected override void NavigateToBack()
        {
            if (ShowSearch)
            {
                ShowSearch = false;
                return;
            }
            base.NavigateToBack();
        }

        private async Task SetAchievements()
        {
            var achievements = await AchievementService.GetAchievements();
            Achievements = AllAchievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
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

        private void UpdateAchievements()
        {
            Expression<Func<AchievementModel, bool>> func = Func();
            var achievements = AllAchievements.Where(func.Compile());
            Achievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private Expression<Func<AchievementModel, bool>> Func()
        {
            Expression<Func<AchievementModel, bool>> expSearch;
            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower()) ||
                I18n.T(it.Description ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());

            return expSearch;
        }
    }
}
