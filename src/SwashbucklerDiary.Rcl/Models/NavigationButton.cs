namespace SwashbucklerDiary.Rcl.Models
{
    public class NavigationButton : DynamicListItem
    {
        public int Index { get; set; }

        public string SelectedIcon { get; set; }

        public string NotSelectedIcon { get; set; }

        public NavigationButton(object receiver, int index, string text, string notSelectedIcon, string selectedIcon, Func<NavigationButton, string> funcIcon, Func<Task> onClick) : base(receiver, text, icon: null, onClick)
        {
            Index = index;
            SelectedIcon = selectedIcon;
            NotSelectedIcon = notSelectedIcon;
            _icon = (Func<string>)(() => funcIcon.Invoke(this));
        }

        public NavigationButton(object receiver, int index, string text, string notSelectedIcon, string selectedIcon, Func<NavigationButton, string> funcIcon, Action onClick) : base(receiver, text, icon: null, onClick)
        {
            Index = index;
            SelectedIcon = selectedIcon;
            NotSelectedIcon = notSelectedIcon;
            _icon = (Func<string>)(() => funcIcon.Invoke(this));
        }
    }
}
