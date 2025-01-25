namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        // from https://github.com/jsuarezruiz/maui-linux/blob/main/src/Essentials/src/Clipboard/Clipboard.Gtk.cs
        static readonly Gdk.Atom clipboardAtom = Gdk.Atom.Intern("CLIPBOARD", false);

        public Task SetClipboardAsync(string text)
        {
            var clipboard = global::Gtk.Clipboard.Get(clipboardAtom);
            clipboard.Text = text;

            return Task.FromResult(0);
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
