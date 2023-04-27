using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Models
{
    public class NavigationButton : ListItemModel
    {
        public string? SelectedIcon { get; set; }

        public NavigationButton(string text, string icon, string selectedIcon, EventCallback onClick) : base(text, icon, onClick)
        {
            SelectedIcon = selectedIcon;
        }
    }
}
