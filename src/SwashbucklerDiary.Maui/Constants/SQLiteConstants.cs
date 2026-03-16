using SwashbucklerDiary.Maui.Essentials;

namespace SwashbucklerDiary.Maui
{
    public class SQLiteConstants : Rcl.SQLiteConstants
    {
        public readonly static string DatabasePath = Path.Combine(AppFileSystem.Default.AppDataDirectory, MainDatabaseFilename);

        public static readonly string PrivacyDatabasePath = Path.Combine(AppFileSystem.Default.AppDataDirectory, PrivacyDatabaseFilename);

        public readonly static string LogDatabasePath = Path.Combine(AppFileSystem.Default.AppDataDirectory, LogDatabaseFilename);

        public readonly static string ConnectionString = GetConnectionString(DatabasePath);

        public readonly static string PrivacyConnectionString = GetConnectionString(PrivacyDatabasePath);

        public readonly static string LogConnectionString = GetConnectionString(LogDatabasePath);
    }
}
