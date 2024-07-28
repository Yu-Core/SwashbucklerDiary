namespace SwashbucklerDiary.Shared
{
    public class BaseModel
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
