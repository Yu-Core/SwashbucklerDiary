namespace SwashbucklerDiary.Models
{
    public class SearchForm
    {
        public bool ShowSearch { get; set; }
        public string? Search { get; set; }
        public DateFilterForm DateFilter { get; set; } = new();
    }
}
