namespace SwashbucklerDiary.Shared
{
    public class DiaryModel : BaseModel
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Mood { get; set; }

        public string? Weather { get; set; }

        public string? Location { get; set; }

        public bool Top { get; set; }

        [Obsolete("Privacy mode no requires it after 1.3.9")]
        public bool Private { get; set; }

        /// <summary>
        /// It was added later, so it may be null in SQL. 
        /// </summary>
        public bool Template { get; set; }

        public List<TagModel>? Tags { get; set; }

        public List<ResourceModel>? Resources { get; set; }
    }
}
