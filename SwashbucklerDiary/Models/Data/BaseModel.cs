using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class BaseModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        [SugarColumn(InsertSql = "DATETIME('now','localtime')")]// getdate() now() sysdate
        public DateTime CreateTime { get; set; }
        [SugarColumn(InsertSql = "DATETIME('now','localtime')", UpdateSql = "DATETIME('now','localtime')")]// getdate() now() sysdate
        public DateTime UpdateTime { get; set; }
    }
}
