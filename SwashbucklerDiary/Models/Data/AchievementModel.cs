namespace SwashbucklerDiary.Models.Data
{
    public class AchievementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AchievementType Type { get; set; }
        public string Description { get; set; }
        public int Steps { get; set; }
        public UserAchievementModel? UserAchievement { get; set; }

        public AchievementModel(int id, string name, AchievementType type, string description, int steps)
        {
            Id = id;
            Name = name;
            Type = type;
            Description = description;
            Steps = steps;
        }
    }

    public enum AchievementType
    {
        Diary, Word, Tag, Avatar, NickName, Sign, SourceCode, Log, Share, Export
    }
}
