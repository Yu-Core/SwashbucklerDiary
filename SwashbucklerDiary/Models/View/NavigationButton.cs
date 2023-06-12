namespace SwashbucklerDiary.Models
{
    public class NavigationButton : DynamicListItem
    {
        public string? SelectedIcon { get; set; }

        public NavigationButton(object receiver, string text, string icon, string selectedIcon, Func<Task> onClick) : base(receiver, text, icon, onClick)
        {
            SelectedIcon = selectedIcon;
        }
    }
}
