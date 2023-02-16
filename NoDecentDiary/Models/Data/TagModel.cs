using SQLite;

namespace NoDecentDiary.Models
{
    public class TagModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
