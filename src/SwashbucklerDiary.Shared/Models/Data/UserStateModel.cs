namespace SwashbucklerDiary.Shared
{
    public class UserStateModel : BaseModel
    {
        public Achievement Type { get; set; }

        public int Count { get; set; }
    }
}
