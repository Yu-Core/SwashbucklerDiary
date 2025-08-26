namespace SwashbucklerDiary.Shared
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> DeepMerge(
            this Dictionary<string, object> first,
            Dictionary<string, object> second)
        {
            var result = new Dictionary<string, object>(first);
            foreach (var kvp in second)
            {
                // 如果两个字典中相同键的值都是字典，则递归合并
                if (result.TryGetValue(kvp.Key, out var existing) &&
                    existing is Dictionary<string, object> existingDict &&
                    kvp.Value is Dictionary<string, object> newDict)
                {
                    result[kvp.Key] = existingDict.DeepMerge(newDict);
                }
                else
                {
                    result[kvp.Key] = kvp.Value; // 覆盖或新增
                }
            }
            return result;
        }
    }
}
