namespace SwashbucklerDiary.Rcl.Models
{
    public class NavigationButton
    {
        public string Text { get; set; }

        public string SelectedIcon { get; set; }

        public string NotSelectedIcon { get; set; }

        public string Href { get; set; }

        public Func<Task> OnClick { get; set; } = default!;

        public NavigationButton(string text, string notSelectedIcon, string selectedIcon,string href)
        {
            Text = text;
            SelectedIcon = selectedIcon;
            NotSelectedIcon = notSelectedIcon;
            Href = href;
        }
    }
}
