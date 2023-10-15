using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public class CardListComponentBase<T> : ImportantComponentBase
    {
        protected List<T> _value = new();

        protected string? SortItem;

        protected Dictionary<string, Func<IEnumerable<T>, IEnumerable<T>>> SortOptions = new();

        [Parameter]
        public virtual List<T> Value
        {
            get => Sort(_value).ToList();
            set => _value = value;
        }

        [Parameter]
        public bool ShowSort { get; set; }

        [Parameter]
        public EventCallback<bool> ShowSortChanged { get; set; }

        protected Dictionary<string, string> SortItems => SortOptions.ToDictionary(it => it.Key, it => it.Key);

        protected virtual IEnumerable<T> Sort(IEnumerable<T> value)
        {
            if (SortItem is null || !SortOptions.TryGetValue(SortItem, out Func<IEnumerable<T>, IEnumerable<T>>? sortOption))
            {
                return value;
            }

            return sortOption.Invoke(value);
        }
    }
}
