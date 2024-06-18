namespace SwashbucklerDiary.Rcl.Models
{
    public class TabListItem
    {
        public TabListItem(string text, string queryParameterValue)
        {
            Text = text;
            QueryParameterValue = queryParameterValue;
        }

        public string? Text { get; set; }

        public string? QueryParameterValue { get; set; }
    }
}
