namespace SwashbucklerDiary.WebAssembly
{
    public class SQLiteConstants
    {
        public const string DatabaseFilename = "SwashbucklerDiary.db3";

        public const string PrivacyDatabaseFilename = "SwashbucklerDiary.privacy.db3";

        public readonly static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public readonly static string PrivacyDatabasePath = Path.Combine(FileSystem.AppDataDirectory, PrivacyDatabaseFilename);
    }
}
