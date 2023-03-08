using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class BaseModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        [SugarColumn(InsertServerTime = true, IsOnlyIgnoreUpdate = true)]// getdate() now() sysdate
        public DateTime CreateTime { get; set; }
        [SugarColumn(InsertServerTime = true, UpdateServerTime = true)]// getdate() now() sysdate
        public DateTime UpdateTime { get; set; }
    }
}
