using Microsoft.AspNetCore.Components;
using OneOf;

namespace SwashbucklerDiary.Models
{
    public class DynamicListItem
    {
        private OneOf<Func<string>, string?> OneOfText;
        private OneOf<Func<string>, string?> OneOfIcon;
        private Func<bool>? FuncShow;

        public string? Text
        {
            get => OneOfText.IsT0 ? OneOfText.AsT0.Invoke() : OneOfText.AsT1;
            set => OneOfText = value;
        }
        public string? Icon
        {
            get => OneOfIcon.IsT0 ? OneOfIcon.AsT0.Invoke() : OneOfIcon.AsT1;
            set => OneOfIcon = value;
        }
        public bool Show
        {
            get => FuncShow is null || FuncShow.Invoke();
        }
        public EventCallback OnClick { get; set; }

        private DynamicListItem(object receiver, string? t = null, Func<string>? tFunc = null, string? i = null, Func<string>? iFunc = null, Action? oAction = null, Func<Task>? oFunc = null, Func<bool>? show = null)
        {
            FuncShow = show;
            OneOfText = t != null ? t : tFunc ?? throw new("At least one is not empty"); ;
            OneOfIcon = i != null ? i : iFunc ?? throw new("At least one is not empty"); ;

            if (oFunc != null)
            {
                OnClick = EventCallback.Factory.Create(receiver, oFunc);
            }

            if (oAction != null)
            {
                OnClick = EventCallback.Factory.Create(receiver, oAction);
            }
        }
        public DynamicListItem(object receiver, string? text, string? icon, Action? onClickAction, Func<bool>? show = null) : this(receiver, t: text, i: icon, oAction: onClickAction, show: show)
        {
        }

        public DynamicListItem(object receiver, string? text, string? icon, Func<Task>? onClickFunc, Func<bool>? show = null) : this(receiver, t: text, i: icon, oFunc: onClickFunc, show: show)
        {
        }

        public DynamicListItem(object receiver, string? text, Func<string>? iconFunc, Action? onClickAction, Func<bool>? show = null) : this(receiver, t: text, iFunc: iconFunc, oAction: onClickAction, show: show)
        {
        }
        public DynamicListItem(object receiver, string? text, Func<string>? iconFunc, Func<Task>? onClickFunc, Func<bool>? show = null) : this(receiver, t: text, iFunc: iconFunc, oFunc: onClickFunc, show: show)
        {
        }
        public DynamicListItem(object receiver, Func<string>? textFunc, string? icon, Action? onClickAction, Func<bool>? show = null) : this(receiver, tFunc: textFunc, i: icon, oAction: onClickAction, show: show)
        {
        }

        public DynamicListItem(object receiver, Func<string>? textFunc, string? icon, Func<Task>? onClickFunc, Func<bool>? show = null) : this(receiver, tFunc: textFunc, i: icon, oFunc: onClickFunc, show: show)
        {
        }

        public DynamicListItem(object receiver, Func<string>? textFunc, Func<string>? iconFunc, Action? onClickAction, Func<bool>? show = null) : this(receiver, tFunc: textFunc, iFunc: iconFunc, oAction: onClickAction, show: show)
        {
        }
        public DynamicListItem(object receiver, Func<string>? textFunc, Func<string>? iconFunc, Func<Task>? onClickFunc, Func<bool>? show = null) : this(receiver, tFunc: textFunc, iFunc: iconFunc, oFunc: onClickFunc, show: show)
        {
        }
    }
}
