namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task ShareFile(string title, string path)
        {
            return Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(path)
            });
        }

        public Task ShareText(string title, string text)
        {
            return Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = title,
                Text = text
            });
        }
    }
}
