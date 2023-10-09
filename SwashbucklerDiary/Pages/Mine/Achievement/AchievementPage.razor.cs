using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class AchievementPage : ImportantComponentBase
    {
        private string? Search;

        private bool ShowSearch;

        private List<AchievementModel> AllAchievements = new();

        private List<AchievementModel> Achievements = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadAchievements();
            await base.OnInitializedAsync();
        }

        protected override Task NavigateToBack()
        {
            if (ShowSearch)
            {
                ShowSearch = false;
                return Task.CompletedTask;
            }

            return base.NavigateToBack();
        }

        private async Task LoadAchievements()
        {
            var achievements = await AchievementService.GetAchievements();
            Achievements = AllAchievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private void UpdateAchievements()
        {
            Expression<Func<AchievementModel, bool>> exp = GetExpression();
            var achievements = AllAchievements.Where(exp.Compile());
            Achievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private Expression<Func<AchievementModel, bool>> GetExpression()
        {
            Expression<Func<AchievementModel, bool>> expSearch;
            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower()) ||
                I18n.T(it.Description ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());

            return expSearch;
        }
    }
}
