namespace SwashbucklerDiary.Models
{
    public class AchievementModel
    {
        public string Name { get; set; }
        public AchievementType Type { get; set; }
        public string Description { get; set; }
        public int Steps { get; set; }
        public UserAchievementModel UserAchievement { get; set; } = default!;

        public AchievementModel(AchievementType type, int steps)
        {
            Name = $"Achievement.{Enum.GetName(typeof(AchievementType), type)}.{steps}.Name";
            Type = type;
            Description = $"Achievement.{Enum.GetName(typeof(AchievementType), type)}.{steps}.Description";
            Steps = steps;
        }

        public AchievementModel(AchievementModel achievement,UserAchievementModel userAchievement)
        {
            Name = achievement.Name;
            Type = achievement.Type;
            Description = achievement.Description;
            Steps = achievement.Steps;
            UserAchievement = userAchievement;
        }
    }
}
