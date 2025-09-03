namespace SwashbucklerDiary.Rcl.Pages
{
    public class SearchFilterConditionForm
    {
        public FilterCondition Tags { get; set; }
        public FilterCondition Weathers { get; set; } = FilterCondition.AnyOne;
        public FilterCondition Moods { get; set; } = FilterCondition.AnyOne;
        public FilterCondition Locations { get; set; } = FilterCondition.AnyOne;
        public FilterCondition FileTypes { get; set; }
    }
}
