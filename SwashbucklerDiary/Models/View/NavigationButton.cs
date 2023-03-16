namespace SwashbucklerDiary.Models
{
    public class NavigationButton : ViewListItem
    {
        public string? SelectedIcon { get; set; }

        public NavigationButton(string text, string icon, string selectedIcon, Action action) : base(text, icon, action)
        {
            SelectedIcon = selectedIcon;
        }
    }
}
