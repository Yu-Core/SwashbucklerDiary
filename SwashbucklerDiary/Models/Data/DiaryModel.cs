using SQLite;

namespace SwashbucklerDiary.Models
{
    public class DiaryModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public string? Location { get; set; }
        public bool Top { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
