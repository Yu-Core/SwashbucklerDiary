using NoDecentDiary.Models;

namespace NoDecentDiary.Pages
{
    public partial class PageWrite
    {
        private readonly List<string> _weathers = new List<string>()
        {
            "晴","阴","小雨","中雨","大雨","小雪","中雪","大雪","雾",
        };
        private string? _weather;
        private bool showMenu;
        private bool showTitle;
        private List<TagModel> SelectedTags = new List<TagModel>()
        {
            new TagModel(){Name="标签"}
        };

        private void RemoveSelectedTag(TagModel tag)
        {
            int index = SelectedTags.IndexOf(tag);
            if (index > -1)
            {
                SelectedTags.RemoveAt(index);
            }
        }
    }
}
