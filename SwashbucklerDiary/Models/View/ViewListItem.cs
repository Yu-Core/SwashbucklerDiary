namespace SwashbucklerDiary.Models
{
    public class ViewListItem
    {
        public ViewListItem(string text, string icon, Action action)
        {
            Text = text;
            Icon = icon;
            Action = action;
        }
        public string Text { get; set; }
        public string Icon { get; set; }
        public Action Action { get; set; }
    }
}
