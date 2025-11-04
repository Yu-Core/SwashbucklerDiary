using Microsoft.Data.Sqlite;

namespace SwashbucklerDiary.Gtk
{
    public class SQLiteConstants : Rcl.SQLiteConstants
    {
        public readonly static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public readonly static string PrivacyDatabasePath = Path.Combine(FileSystem.AppDataDirectory, PrivacyDatabaseFilename);

        public readonly static string ConnectionString = GetConnectionString(DatabasePath);

        public readonly static string PrivacyConnectionString = GetConnectionString(PrivacyDatabasePath);
    }
}
