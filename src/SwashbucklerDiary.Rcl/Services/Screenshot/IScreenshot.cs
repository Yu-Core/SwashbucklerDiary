namespace SwashbucklerDiary.Rcl.Services
{
    public interface IScreenshot
    {
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="selector">元素选择器</param>
        /// <returns>文件路径</returns>
        Task<string?> CaptureAsync(string selector);
    }
}
