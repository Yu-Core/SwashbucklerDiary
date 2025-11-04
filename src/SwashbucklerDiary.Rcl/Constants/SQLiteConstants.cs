using Microsoft.Data.Sqlite;

namespace SwashbucklerDiary.Rcl
{
    public class SQLiteConstants
    {
        public const string DatabaseFilename = "SwashbucklerDiary.db3";

        public const string PrivacyDatabaseFilename = "SwashbucklerDiary.privacy.db3";

        protected static string GetConnectionString(string dataSource)
        {
            return new SqliteConnectionStringBuilder()
            {
                DataSource = dataSource,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared

            }.ToString();
        }
    }
}
