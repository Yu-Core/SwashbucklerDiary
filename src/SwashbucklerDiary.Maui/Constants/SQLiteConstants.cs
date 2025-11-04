using SwashbucklerDiary.Maui.Essentials;

namespace SwashbucklerDiary.Maui
{
    public class SQLiteConstants : Rcl.SQLiteConstants
    {
        public readonly static string DatabasePath = Path.Combine(AppFileSystem.Default.AppDataDirectory, DatabaseFilename);

        public static readonly string PrivacyDatabasePath = Path.Combine(AppFileSystem.Default.AppDataDirectory, PrivacyDatabaseFilename);

        public readonly static string ConnectionString = GetConnectionString(DatabasePath);

        public readonly static string PrivacyConnectionString = GetConnectionString(PrivacyDatabasePath);
    }
}
