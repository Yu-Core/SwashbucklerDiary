namespace SwashbucklerDiary.Shared
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> WhereIF<T>(this IEnumerable<T> thisValue, bool isOk, Func<T, bool> predicate)
        {
            if (isOk)
            {
                return thisValue.Where(predicate);
            }

            return thisValue;
        }
    }
}
