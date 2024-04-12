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

        private List<AchievementModel> _achievements = [];

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await LoadAchievements();
                StateHasChanged();
            }
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
            _achievements = allAchievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private void UpdateAchievements()
        {
            Expression<Func<AchievementModel, bool>> exp = GetExpression();
            var achievements = allAchievements.Where(exp.Compile());
            _achievements = achievements.OrderByDescending(it => it.UserAchievement.IsCompleted).ToList();
        }

        private Expression<Func<AchievementModel, bool>> GetExpression()
        {
            Expression<Func<AchievementModel, bool>> expSearch
                = it => I18n.T(it.Name ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                || I18n.T(it.Description ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);

            return expSearch;
        }
    }
}
