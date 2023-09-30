using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class AchievementCard : MyComponentBase
    {
        [Parameter]
        public AchievementModel Value { get; set; } = default!;

        private string IconColor
            => Value.UserAchievement.IsCompleted && Light ? "black" : "";

        private string Icon
            => Value.UserAchievement.IsCompleted ? "mdi-trophy" : "mdi-trophy-outline";

        private int ProgressRate
            => (int)((double)Value.UserAchievement.CompleteRate / Value.Steps * 100);

        private string ProgressRateColor
            => Value.UserAchievement.IsCompleted && Light ? "#2e2e2e" : "grey lighten-1";

        private string ProgressRateTextColor 
            => Value.UserAchievement.IsCompleted && Light ? "white--text" : "";

        private string ProgressRateText => GetProgressRateText(Value);

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
