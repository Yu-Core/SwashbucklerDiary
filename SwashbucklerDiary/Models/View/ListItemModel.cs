using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Models
{
    public class ListItemModel
    {
        private string? _text;
        private string? _icon;
        public string? Text
        {
            get
            {
                if(TextFunc != null)
                {
                    return TextFunc.Invoke();
                }
                return _text;
            }
            set => _text = value;
        }
        public string? Icon
        {
            get
            {
                if (IconFunc != null)
                {
                    return IconFunc.Invoke();
                }
                return _icon;
            }
            set => _icon = value;
        }
        public EventCallback OnClick { get; set; }

        public Func<string>? TextFunc { get; set; }
        public Func<string>? IconFunc { get; set; }

        public ListItemModel(string text, string icon, EventCallback onClick)
        {
            _text = text;
            _icon = icon;
            OnClick = onClick;
        }

        public ListItemModel(Func<string> text, Func<string> icon, EventCallback onClick)
        {
            TextFunc = text;
            IconFunc = icon;
            OnClick = onClick;
        }

        public ListItemModel(Func<string> text, string icon, EventCallback onClick)
        {
            TextFunc = text;
            _icon = icon;
            OnClick = onClick;
        }
    }
}
