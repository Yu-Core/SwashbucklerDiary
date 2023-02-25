namespace SwashbucklerDiary.Models
{
    public class UserStateModel : BaseModel
    {
        public AchievementType Type { get; set; }
        public int Count { get; set; }
    }
}
