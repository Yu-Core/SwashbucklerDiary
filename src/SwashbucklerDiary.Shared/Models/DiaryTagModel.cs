namespace SwashbucklerDiary.Shared
{
    public class DiaryTagModel : BaseModel
    {
        public Guid DiaryId { get; set; }

        public Guid TagId { get; set; }
    }
}
