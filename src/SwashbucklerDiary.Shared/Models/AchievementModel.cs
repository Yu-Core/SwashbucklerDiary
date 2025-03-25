namespace SwashbucklerDiary.Shared
{
    public class AchievementModel
    {
        public string? Name { get; set; }

        public Achievement Kind { get; set; }

        public string? Description { get; set; }

        public int Steps { get; set; }

        public UserAchievementModel UserAchievement { get; set; } = default!;

        public AchievementModel()
        {
        }
        public AchievementModel(AchievementModel achievement,UserAchievementModel userAchievement)
        {
            Name = achievement.Name;
            Kind = achievement.Kind;
            Description = achievement.Description;
            Steps = achievement.Steps;
            UserAchievement = userAchievement;
        }
    }
}
