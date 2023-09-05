using Microsoft.Data.Sqlite;

namespace SwashbucklerDiary.Config
{
    public static class SQLiteConstants
    {
        public const string DatabaseFilename = "SwashbucklerDiary.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public readonly static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public readonly static string ConnectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared

        }.ToString();
    }
}
