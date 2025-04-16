using SwashbucklerDiary.Rcl.Services;
using System.Text;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class II18nServiceExtensions
    {
        const string appFeaturePathSeparators = " / ";
        const string orderSeparators = " - ";
        static readonly string[] timeFormatSeparators = ["/", ":", " "];

        public static string ToWeek(this II18nService i18n, DateTime? dateTime = null)
        {
            return i18n.T((dateTime ?? DateTime.Now).DayOfWeek.ToString());
        }

        public static string TForAppFeaturePath(this II18nService i18n, string? key)
            => ReplaceTextWithSeparators(key, appFeaturePathSeparators, it => i18n.T(it));

        public static string TForTimeFormat(this II18nService i18n, string key)
            => ReplaceTextWithSeparators(key, timeFormatSeparators, it => i18n.T(it));

        public static string TForOrder(this II18nService i18n, string key)
            => ReplaceTextWithSeparators(key, orderSeparators, it => i18n.T(it));

        public static string ReplaceTextWithSeparators(string? key, string separator, Func<string, string> func)
            => ReplaceTextWithSeparators(key, [separator], func);

        public static string ReplaceTextWithSeparators(string? text, string[] separators, Func<string, string> func)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (separators.Length == 0)
            {
                return func.Invoke(text);
            }

            if (separators.Length == 1)
            {
                var separator = separators[0];
                var splits = text.Split(separator).Select(func);
                return string.Join(separator, splits);
            }

            StringBuilder result = new StringBuilder();
            int currentPos = 0; // 当前扫描位置
            int inputLength = text.Length;

            while (currentPos < inputLength)
            {
                // 查找下一个分隔符的起始位置
                int nextSeparatorIndex = -1;
                string foundSeparator = string.Empty;

                // 检查所有可能的分隔符，选择最先出现的
                foreach (var sep in separators)
                {
                    int index = text.IndexOf(sep, currentPos);
                    if (index != -1 && (nextSeparatorIndex == -1 || index < nextSeparatorIndex))
                    {
                        nextSeparatorIndex = index;
                        foundSeparator = sep;
                    }
                }

                // 如果没有找到更多分隔符，剩余部分全部是文本
                if (nextSeparatorIndex == -1)
                {
                    string remainingText = text.Substring(currentPos);
                    result.Append(func.Invoke(remainingText));
                    break;
                }

                // 提取当前文本（分隔符之前的部分）
                if (nextSeparatorIndex > currentPos)
                {
                    string textBeforeSeparator = text.Substring(currentPos, nextSeparatorIndex - currentPos);
                    result.Append(func.Invoke(textBeforeSeparator));
                }

                // 添加分隔符（不处理）
                result.Append(foundSeparator);

                // 移动当前位置到分隔符之后
                currentPos = nextSeparatorIndex + foundSeparator.Length;
            }

            return result.ToString();
        }
    }
}
