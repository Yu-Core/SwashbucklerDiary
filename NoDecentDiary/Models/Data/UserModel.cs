using SQLite;

namespace NoDecentDiary.Models
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
