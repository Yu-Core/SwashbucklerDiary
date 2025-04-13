using Masa.Blazor;

namespace SwashbucklerDiary.Rcl.Models
{
    public class NavigationButton
    {
        public string Text { get; set; }

        public string Icon { get; set; }

        public string Href { get; set; }

        public NavigationButton(string text, string icon, string href)
        {
            Text = text;
            Icon = icon;
            Href = href;
        }
    }
}
