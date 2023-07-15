namespace SwashbucklerDiary.Models
{
    public class AppFunction
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public string? Path { get; set; }
        public string? Href { get; set; }
        public bool ConditionalDisplay { get; set; } 
        public bool Privacy { get; set; }
    }
}
