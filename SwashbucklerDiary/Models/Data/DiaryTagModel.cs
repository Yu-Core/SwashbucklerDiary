using SQLite;

namespace SwashbucklerDiary.Models
{
    public class DiaryTagModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int DiaryId { get; set; }
        public int TagId { get; set; }
    }
}
