using DocumentFormat.OpenXml.Office2010.Excel;
using Masa.Blazor;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class DiaryModelExtensions
    {
        const int maxDisplayTitleLength = 200;
        const int maxDisplayContentLength = 2000;

        public static string CreateCopyContent(this DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content ?? string.Empty;
            }

            return diary.Title + "\n" + diary.Content;
        }

        public static string? GetDisplayTitle(this DiaryModel diary)
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

                int length = GetDisplayTitleLengthFromContent(diary.Content, maxDisplayTitleLength);
                if (length > 0)
                {
                    return diary.Content.Substring(0, length);
                }
                else
                {
                    return SubText(diary.Content, 0, maxDisplayTitleLength);
                }
            }
        }

        public static string? GetDisplayContent(this DiaryModel diary, int displayTitleLength)
        {
            if (string.IsNullOrWhiteSpace(diary.Content))
            {
                return string.Empty;
            }

            int startIndex = 0;
            if (string.IsNullOrWhiteSpace(diary.Title) && displayTitleLength < diary.Content.Length)
            {
                startIndex = displayTitleLength;
            }

            string content = SubText(diary.Content, startIndex, maxDisplayContentLength);
            return content;
        }

        public static int GetWordCount(this DiaryModel diary)
        {
            var wordCount = new WordCount();
            if (string.IsNullOrWhiteSpace(diary.Content))
            {
                return 0;
            }

            wordCount.GetCountWords(diary.Content);
            return wordCount.TotalWordCount;
        }

        public static int GetWordCount(this List<DiaryModel> diaries)
        {
            return diaries.Sum(d => d.GetWordCount());
        }

        public static string GetReferenceText(this DiaryModel diary, II18nService i18n)
        {
            return $"[{i18n.T(diary.Template ? "Template reference" : "Diary reference")}](read/{diary.Id})";
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

        private static int GetDisplayTitleLengthFromContent(string content, int maxLength)
        {
            maxLength = Math.Min(content.Length, maxLength);
            char[] separators = [']', '。', '\n']; // 定义分隔符
            int index = content.IndexOfAny(separators, 0, maxLength);
            if (index == -1 || index == content.Length - 1)
            {
                index = content.IndexOf(". ", 0, maxLength);
                if (index > -1)
                {
                    index++;
                }
            }

            return (index != content.Length - 1 ? index : -1) + 1;
        }
    }
}
