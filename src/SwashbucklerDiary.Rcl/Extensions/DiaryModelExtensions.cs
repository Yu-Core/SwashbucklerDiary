using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class DiaryModelExtensions
    {
        const int maxDisplayTitleLength = 100;
        const int maxDisplayContentLength = 300;

        public static string CreateCopyContent(this DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content ?? string.Empty;
            }

            return diary.Title + "\n" + diary.Content;
        }

        public static (string Title, string Content) GetDisplayTitleAndContent(this DiaryModel diary, bool notExtractTitle)
        {
            string titleResult = string.Empty;
            string contentResult = string.Empty;

            if (string.IsNullOrWhiteSpace(diary.Content))
                return (string.Empty, string.Empty);

            var content = diary.Content.Trim();

            // 如果原本有标题
            if (notExtractTitle || !string.IsNullOrWhiteSpace(diary.Title))
            {
                titleResult = Truncate(diary.Title, maxDisplayTitleLength);
                contentResult = Truncate(content, maxDisplayContentLength);
                return (titleResult, contentResult);
            }

            // 没标题时，手动找第一行
            int newLineIndex = content.IndexOf('\n');

            if (newLineIndex < 0)
            {
                // content 只有一行 -> 不生成标题
                titleResult = string.Empty;
                contentResult = Truncate(content, maxDisplayContentLength);
            }
            else
            {
                // 多行内容 -> 第一行做标题，第二行起做内容
                string firstLine = content[..newLineIndex].Trim();
                string remaining = (newLineIndex + 1 < content.Length)
                    ? content[(newLineIndex + 1)..].Trim()
                    : string.Empty;

                titleResult = Truncate(firstLine, maxDisplayTitleLength);
                contentResult = Truncate(remaining, maxDisplayContentLength);
            }

            return (titleResult, contentResult);
        }

        private static string Truncate(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Length <= maxLength ? text : text[..maxLength] + "...";
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

        public static string GetReferenceText(this DiaryModel diary, II18nService i18n, string? urlScheme, string? hash = null)
        {
            string title = diary.GetDisplayTitleAndContent(false).Title
                ?? i18n.T(diary.Template ? "Template reference" : "Diary reference");
            string url = $"{urlScheme}://read/{diary.Id}{hash}";
            return $"[{title}]({url})";
        }
    }
}
