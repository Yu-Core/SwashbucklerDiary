using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AchievementPage : ImportantComponentBase
    {
        private string? search;

        private bool showSearch;

        private List<AchievementModel> allAchievements = [];

        private List<AchievementModel> achievements = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadAchievements();
            await base.OnInitializedAsync();
        }

        protected override Task NavigateToBack()
        {
            if (showSearch)
            {
                showSearch = false;
                return Task.CompletedTask;
            }

            return base.NavigateToBack();
        }

        private async Task LoadAchievements()
        {
            var achievements = await AchievementService.GetAchievements();
            this.achievements = allAchievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private void UpdateAchievements()
        {
            Expression<Func<AchievementModel, bool>> exp = GetExpression();
            var achievements = allAchievements.Where(exp.Compile());
            this.achievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private Expression<Func<AchievementModel, bool>> GetExpression()
        {
            Expression<Func<AchievementModel, bool>> expSearch;
            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower()) ||
                I18n.T(it.Description ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower());

            return expSearch;
        }
    }
}
