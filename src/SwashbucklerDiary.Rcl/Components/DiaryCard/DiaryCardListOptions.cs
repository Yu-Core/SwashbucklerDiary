using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public class DiaryCardListOptions
    {
        public DiaryModel SelectedItemValue { get; set; } = new();

        public string? DateFormat { get; set; }
    }
}
