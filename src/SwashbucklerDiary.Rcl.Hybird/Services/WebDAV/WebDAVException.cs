namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDAVException : Exception
    {
        public WebDAVException(string? message) : base(message)
        {
        }
    }
}
