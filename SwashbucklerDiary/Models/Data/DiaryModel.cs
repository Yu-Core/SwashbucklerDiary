using SqlSugar;

namespace SwashbucklerDiary.Models
{
    public class DiaryModel : BaseModel
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public string? Location { get; set; }
        public bool Top { get; set; }
        public bool Private { get; set; }

        [Navigate(typeof(DiaryTagModel), nameof(DiaryTagModel.DiaryId), nameof(DiaryTagModel.TagId))]
        public List<TagModel>? Tags { get; set; }
    }
}
