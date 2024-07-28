namespace SwashbucklerDiary.Shared
{
    public class DiaryResourceModel : BaseModel
    {
        public Guid DiaryId { get; set; }

        public string? ResourceUri { get; set; }
    }
}
