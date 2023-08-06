using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class ResourceModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string? ResourceUri { get; set; }
        public ResourceType ResourceType { get; set; }
    }
}
