using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AchievementCard : MyComponentBase
    {
        [Parameter]
        public AchievementModel Value { get; set; } = default!;

        private string IconColor
            => Value.UserAchievement.IsCompleted ? "rgba(var(--m-theme-on-surface))" : "";

        private string Icon
            => Value.UserAchievement.IsCompleted ? "trophy-fill" : "trophy";

        private int ProgressRate
            => (int)((double)Value.UserAchievement.CompleteRate / Value.Steps * 100);

        private string ProgressRateColor
            => Value.UserAchievement.IsCompleted ? "rgba(var(--m-theme-inverse-surface))" : "rgba(var(--m-theme-surface-container-highest))";

        private string ProgressRateTextColor
            => Value.UserAchievement.IsCompleted ? "rgba(var(--m-theme-inverse-on-surface))" : "";

        private string ProgressRateText => GetProgressRateText(Value);

        private string GetProgressRateText(AchievementModel achievement)
        {
            if (achievement.UserAchievement.IsCompleted)
            {
                return $"{I18n.T("Completed")} {achievement.UserAchievement.CompletedTime:d}";
            }
            else
            {
                return $"{achievement.UserAchievement.CompleteRate} / {achievement.Steps}";
            }
        }
    }
}
