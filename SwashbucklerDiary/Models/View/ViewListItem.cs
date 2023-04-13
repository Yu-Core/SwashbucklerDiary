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
        public MulticastDelegate Delegate { get; set; }

        public Func<string>? TextFunc { get; set; }
        public Func<string>? IconFunc { get; set; }

        public ListItemModel(string text, string icon, Action action)
        {
            _text = text;
            _icon = icon;
            Delegate = action;
        }

        public ListItemModel(string text, string icon, Func<Task> func)
        {
            _text = text;
            _icon = icon;
            Delegate = func;
        }

        public ListItemModel(Func<string> text, Func<string> icon, Action action)
        {
            TextFunc = text;
            IconFunc = icon;
            Delegate = action;
        }

        public ListItemModel(Func<string> text, Func<string> icon, Func<Task> func)
        {
            TextFunc = text;
            IconFunc = icon;
            Delegate = func;
        }

        public ListItemModel(Func<string> text, string icon, Action action)
        {
            TextFunc = text;
            _icon = icon;
            Delegate = action;
        }

        public ListItemModel(Func<string> text, string icon, Func<Task> func)
        {
            TextFunc = text;
            _icon = icon;
            Delegate = func;
        }
    }
}
