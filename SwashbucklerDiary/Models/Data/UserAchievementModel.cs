using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class UserAchievementModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int AchievementId { get; set; }
        public int CompleteRate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedTime { get; set; }
    }
}
