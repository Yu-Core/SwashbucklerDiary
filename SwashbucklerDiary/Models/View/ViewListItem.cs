namespace SwashbucklerDiary.Models
{
    public class ViewListItem
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
        public Action Action { get; set; }

        public Func<string>? TextFunc { get; set; }
        public Func<string>? IconFunc { get; set; }

        public ViewListItem(string text, string icon, Action action)
        {
            _text = text;
            _icon = icon;
            Action = action;
        }

        public ViewListItem(Func<string> text, Func<string> icon, Action action)
        {
            TextFunc = text;
            IconFunc = icon;
            Action = action;
        }
    }
}
