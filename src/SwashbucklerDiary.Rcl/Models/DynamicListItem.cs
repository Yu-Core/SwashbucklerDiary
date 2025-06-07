using Microsoft.AspNetCore.Components;
using OneOf;

namespace SwashbucklerDiary.Rcl.Models
{
    public class DynamicListItem
    {
        protected TFunc<string> _text = default!;

        protected TFunc<string> _icon = default!;

        protected Func<bool>? _funcShow;

        public string? Text
        {
            get => _text.ToT();
        }

        public string? Icon
        {
            get => _icon.ToT();
        }

        public bool Show
        {
            get => _funcShow is null || _funcShow.Invoke();
        }

        public EventCallback OnClick { get; set; }

        public DynamicListItem(DynamicListItem dynamicListItem)
        {
            _text = dynamicListItem._text;
            _icon = dynamicListItem._icon;
            _funcShow = dynamicListItem._funcShow;
            OnClick = dynamicListItem.OnClick;
        }

        protected DynamicListItem(Func<bool>? funcShow)
        {
            _funcShow = funcShow;
        }

        protected DynamicListItem(object receiver, Action actionOnClick, Func<bool>? funcShow) : this(funcShow)
        {
            OnClick = EventCallback.Factory.Create(receiver, actionOnClick);
        }

        protected DynamicListItem(object receiver, Func<Task> funcOnClick, Func<bool>? funcShow) : this(funcShow)
        {
            OnClick = EventCallback.Factory.Create(receiver, funcOnClick);
        }

        public DynamicListItem(object receiver, string text, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, actionOnClick, funcShow)
        {
            _text = text;
        }

        public DynamicListItem(object receiver, Func<string> funcText, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, actionOnClick, funcShow)
        {
            _text = funcText;
        }

        public DynamicListItem(object receiver, string text, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, funcOnClick, funcShow)
        {
            _text = text;
        }

        public DynamicListItem(object receiver, Func<string> funcText, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, funcOnClick, funcShow)
        {
            _text = funcText;
        }

        public DynamicListItem(object receiver, string text, string icon, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, text, actionOnClick, funcShow)
        {
            _icon = icon;
        }

        public DynamicListItem(object receiver, string text, string icon, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, text, funcOnClick, funcShow)
        {
            _icon = icon;
        }

        public DynamicListItem(object receiver, string text, Func<string> funcIcon, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, text, actionOnClick, funcShow)
        {
            _icon = funcIcon;
        }

        public DynamicListItem(object receiver, string text, Func<string> funcIcon, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, text, funcOnClick, funcShow)
        {
            _icon = funcIcon;
        }

        public DynamicListItem(object receiver, Func<string> funcText, string icon, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, funcText, actionOnClick, funcShow)
        {
            _icon = icon;
        }

        public DynamicListItem(object receiver, Func<string> funcText, string icon, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, funcText, funcOnClick, funcShow)
        {
            _icon = icon;
        }

        public DynamicListItem(object receiver, Func<string> funcText, Func<string> funcIcon, Action actionOnClick, Func<bool>? funcShow = null) : this(receiver, funcText, actionOnClick, funcShow)
        {
            _icon = funcIcon;
        }

        public DynamicListItem(object receiver, Func<string> funcText, Func<string> funcIcon, Func<Task> funcOnClick, Func<bool>? funcShow = null) : this(receiver, funcText, funcOnClick, funcShow)
        {
            _icon = funcIcon;
        }
    }

    public class DynamicListItem<T> : DynamicListItem
    {
        public T Value { get; set; }

        public DynamicListItem(object receiver, string text, string icon, Action<T> actionOnClick, T value, Func<bool>? funcShow = null) : base(funcShow)
        {
            _text = text;
            _icon = icon;
            Value = value;
            OnClick = EventCallback.Factory.Create(receiver, () => actionOnClick.Invoke(Value));
        }

        public DynamicListItem(object receiver, string text, string icon, Func<T, Task> funcOnClick, T value, Func<bool>? funcShow = null) : base(funcShow)
        {
            _text = text;
            _icon = icon;
            Value = value;
            OnClick = EventCallback.Factory.Create(receiver, () => funcOnClick.Invoke(Value));
        }
    }

    [GenerateOneOf]
    public partial class TFunc<T> : OneOfBase<T, Func<T>>
    {
        public T ToT() => Match(
            t0 => t0,
            t1 => t1.Invoke()
        );

        public static bool operator ==(TFunc<T>? left, TFunc<T>? right)
        {
            if (Equals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Value == right.Value;
        }

        public static bool operator !=(TFunc<T>? left, TFunc<T>? right)
        {
            if (Equals(left, right))
            {
                return false;
            }

            if (left is null || right is null)
            {
                return true;
            }

            return left.Value != right.Value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
