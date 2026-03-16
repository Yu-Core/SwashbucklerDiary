using Microsoft.Maui.Storage;

namespace SwashbucklerDiary.Gtk
{
    public class SQLiteConstants : Rcl.SQLiteConstants
    {
        public readonly static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, MainDatabaseFilename);

        public readonly static string PrivacyDatabasePath = Path.Combine(FileSystem.AppDataDirectory, PrivacyDatabaseFilename);

        public readonly static string LogDatabasePath = Path.Combine(FileSystem.AppDataDirectory, LogDatabaseFilename);

        public readonly static string ConnectionString = GetConnectionString(DatabasePath);

        public readonly static string PrivacyConnectionString = GetConnectionString(PrivacyDatabasePath);

        public readonly static string LogConnectionString = GetConnectionString(LogDatabasePath);
    }
}
