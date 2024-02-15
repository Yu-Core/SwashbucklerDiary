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
    }
}
