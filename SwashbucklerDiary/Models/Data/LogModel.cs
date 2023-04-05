using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class LogModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        [SugarColumn(ColumnDataType = "varchar(10)")]
        public string? Level { get; set; }
        public string? Exception { get; set; }
        public string? RenderedMessage { get; set; }
        public string? Properties { get; set; }
    }
}
