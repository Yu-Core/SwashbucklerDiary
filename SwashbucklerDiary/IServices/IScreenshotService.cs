namespace SwashbucklerDiary.IServices
{
    public interface IScreenshotService
    {
        Task<string> ScreenshotToBase64(string selector);
    }
}
