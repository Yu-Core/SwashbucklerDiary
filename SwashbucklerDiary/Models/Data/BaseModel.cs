using SqlSugar;
using System.Security.Principal;

namespace SwashbucklerDiary.Models
{
    public class BaseModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
