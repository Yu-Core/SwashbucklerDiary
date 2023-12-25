namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public Task SetClipboard(string text)
            => Clipboard.Default.SetTextAsync(text);

        public Task ShareFileAsync(string title, string path)
        {
            if (!File.Exists(path))
            {
                return Task.CompletedTask;
            }

            return Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(path)
            });
        }

        public Task ShareTextAsync(string title, string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                return Task.CompletedTask;
            }

            return Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = title,
                Text = text
            });
        }
    }
}
