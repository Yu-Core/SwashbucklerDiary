namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task SetClipboardAsync(string text)
        {
            Gdk.Clipboard clipboard = Gdk.Display.GetDefault()!.GetClipboard();
            clipboard.SetText(text ?? string.Empty);
            return Task.CompletedTask;
        }

        public Task ShareFileAsync(string title, string path)
        {
            return Task.CompletedTask;
        }

        public Task ShareTextAsync(string title, string text)
        {
            return Task.CompletedTask;
        }
    }
}
