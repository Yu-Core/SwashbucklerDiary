using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class DiaryModelExtensions
    {
        public static string CreateCopyContent(this DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content ?? string.Empty;
            }

            return diary.Title + "\n" + diary.Content;
        }

        public static string? ExtractTitle(this DiaryModel diary)
        {
            if (!string.IsNullOrWhiteSpace(diary.Title))
            {
                return diary.Title;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(diary.Content))
                {
                    return string.Empty;
                }

                int index = ExtractTitleIndex(diary.Content);
                if (index > -1)
                {
                    return diary.Content.Substring(0, index + 1);
                }
                else
                {
                    return SubText(diary.Content, 0, 200);
                }
            }
        }

        public static string? ExtractText(this DiaryModel diary)
        {
            if (string.IsNullOrWhiteSpace(diary.Content))
            {
                return string.Empty;
            }

            string text = SubText(diary.Content, 0, 2000);
            if (string.IsNullOrWhiteSpace(diary.Title))
            {
                int index = ExtractTitleIndex(diary.Content);
                var subText = SubText(text, index + 1);
                if (subText != string.Empty)
                {
                    return subText;
                }
            }

            return text;
        }

        private static string SubText(string? text, int startIndex, int? length = null)
        {
            if (text == null)
            {
                return string.Empty;
            }

            int textLength = text.Length;

            if (startIndex < 0 || startIndex >= textLength)
            {
                return string.Empty;
            }

            if (length == null)
            {
                length = textLength - startIndex;
            }
            else if (length < 0)
            {
                return string.Empty;
            }

            int endIndex = startIndex + length.Value;

            if (endIndex > textLength)
            {
                endIndex = textLength;
            }

            return text.Substring(startIndex, endIndex - startIndex);
        }

        private static int ExtractTitleIndex(string text)
        {
            char[] separators = [']', '。', '\n']; // 定义分隔符
            int index = text.IndexOfAny(separators);
            if (index == -1 || index == text.Length - 1)
            {
                index = text.IndexOf(". ");
                if (index > -1)
                {
                    index++;
                }
            }

            return index != text.Length - 1 ? index : -1;
        }
    }
}
