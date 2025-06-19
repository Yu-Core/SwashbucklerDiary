using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class DiaryModelExtensions
    {
        const int maxDisplayContentLength = 300;

        public static string CreateCopyContent(this DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content ?? string.Empty;
            }

            return diary.Title + "\n" + diary.Content;
        }

        public static string? GetDisplayContent(this DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Content))
            {
                return string.Empty;
            }

            return TruncateString(diary.Content, maxDisplayContentLength);
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

        private static string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var stringInfo = new StringInfo(input);
            if (stringInfo.LengthInTextElements <= maxLength)
            {
                return input;
            }

            return stringInfo.SubstringByTextElements(0, maxLength) + "...";
        }
    }
}
