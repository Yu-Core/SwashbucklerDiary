using Gtk;

namespace SwashbucklerDiary.Gtk.Essentials;

public static class FileDialogUtils
{
    private static Window DefaultWindow
    {
        get
        {
            if (Application.GetDefault() is Application application && application.ActiveWindow is not null)
            {
                return application.ActiveWindow;
            }

            throw new InvalidOperationException("Application ActiveWindow not exists");
        }
    }

    public static void AddFilter(this FileDialog fileDialog, string name, IEnumerable<string> patterns)
    {
        var fileFilters = Gio.ListStore.New(FileFilter.GetGType());
        var fileFilter = FileFilter.New();
        fileFilter.Name = name;

        foreach (var pattern in patterns)
        {
            fileFilter.AddPattern(pattern);
        }

        fileFilters.Append(fileFilter);
        fileDialog.Filters = fileFilters;
    }

    public static Task<Gio.File?> OpenAsync(this FileDialog fileDialog)
        => fileDialog.OpenAsync(DefaultWindow);

    public static Task<Gio.ListModel?> OpenMultipleAsync(this FileDialog fileDialog)
        => fileDialog.OpenMultipleAsync(DefaultWindow);

    public static Task<Gio.File?> SaveAsync(this FileDialog fileDialog)
        => fileDialog.SaveAsync(DefaultWindow);
}