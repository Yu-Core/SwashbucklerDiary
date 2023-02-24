using SqlSugar;

namespace SwashbucklerDiary.Models.Data
{
    public class UserStateModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public AchievementType Type { get; set; }
        public int Count { get; set; }
    }
}
