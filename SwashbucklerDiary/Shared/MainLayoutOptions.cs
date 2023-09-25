using BlazorComponent;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Shared
{
    public class MainLayoutOptions
    {
        private StringNumber _navigationIndex = 0;

        public StringNumber NavigationIndex
        {
            get => _navigationIndex;
            set
            {
                if (value == _navigationIndex) return;

                _navigationIndex = value;
                Task.Run(() =>
                {
                    NavigationIndexAction?.Invoke();
                });
            }
        }
        public Action? NavigationIndexAction { get; set; }
        public List<NavigationButton> NavigationButtons { get; set; } = new();
    }
}
