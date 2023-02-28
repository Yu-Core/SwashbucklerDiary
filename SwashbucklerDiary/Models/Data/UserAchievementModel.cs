using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class UserAchievementModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string? AchievementName { get; set; }
        public int CompleteRate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedTime { get; set; }
    }
}
