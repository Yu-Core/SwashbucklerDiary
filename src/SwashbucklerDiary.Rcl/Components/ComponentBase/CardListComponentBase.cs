using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class CardListComponentBase<T> : ImportantComponentBase
    {
        protected List<T> _value = [];

        protected string? sortItem;

        protected bool showSort;

        protected Dictionary<string, Func<IEnumerable<T>, IEnumerable<T>>> sortOptions = [];

        [Parameter]
        public virtual List<T> Value
        {
            get => Sort(_value).ToList();
            set => _value = value;
        }

        public void Sort()
        {
            showSort = true;
            InvokeAsync(StateHasChanged);
        }

        protected List<string> SortItems => sortOptions.Keys.ToList();

        protected virtual IEnumerable<T> Sort(IEnumerable<T> value)
        {
            if (sortItem is null || !sortOptions.TryGetValue(sortItem, out Func<IEnumerable<T>, IEnumerable<T>>? sortOption))
            {
                return value;
            }

            return sortOption.Invoke(value);
        }
    }
}
