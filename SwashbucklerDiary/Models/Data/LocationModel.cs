namespace SwashbucklerDiary.Models
{
    public class LocationModel : BaseModel
    {
        public string? Name { get; set; }

        public LocationModel() 
        { 
        }

        public LocationModel(string name)
        {
            Name = name;
        }
    }
}
