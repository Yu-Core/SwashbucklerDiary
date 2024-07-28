namespace SwashbucklerDiary.Shared
{
    public class UserAchievementModel
    {
        public Guid Id { get; set; }

        public string? AchievementName { get; set; }

        public int CompleteRate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CompletedTime { get; set; }
    }
}
