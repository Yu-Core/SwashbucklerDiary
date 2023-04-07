// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Batch;
using Serilog.Sinks.Extensions;
using SQLite;

namespace Serilog.Sinks.SQLite
{
    internal class SQLiteSink : BatchProvider, ILogEventSink
    {
        private readonly string _databasePath; private readonly bool _storeTimestampInUtc; private readonly bool _rollOver; private readonly string _tableName;




        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public SQLiteSink(
            string sqlLiteDbPath,
            string tableName,
            bool storeTimestampInUtc,
            uint batchSize = 100,
            bool rollOver = true) : base(batchSize: (int)batchSize, maxBufferSize: 100_000)
        {
            _databasePath = sqlLiteDbPath;
            _tableName = tableName;
            _storeTimestampInUtc = storeTimestampInUtc;
            _rollOver = rollOver;

            InitializeDatabase();
        }

        #region ILogEvent implementation

        public void Emit(LogEvent logEvent)
        {
            PushEvent(logEvent);
        }

        #endregion

        private void InitializeDatabase()
        {
            var conn = GetSqLiteAsyncConnection();
            CreateSqlTable(conn);
        }

        private SQLiteAsyncConnection GetSqLiteAsyncConnection()
        {
            var sqlConString = new SQLiteConnectionString(_databasePath, true);
            var sqLiteConnection = new SQLiteAsyncConnection(sqlConString);
            return sqLiteConnection;
        }

        private void CreateSqlTable(SQLiteAsyncConnection sqlConnection)
        {
            var colDefs = "id INTEGER PRIMARY KEY AUTOINCREMENT,";
            colDefs += "Timestamp TEXT,";
            colDefs += "Level VARCHAR(10),";
            colDefs += "Exception TEXT,";
            colDefs += "RenderedMessage TEXT,";
            colDefs += "Properties TEXT";

            var sqlCreateText = $"CREATE TABLE IF NOT EXISTS {_tableName} ({colDefs})";

            sqlConnection.ExecuteAsync(sqlCreateText).ConfigureAwait(false);
        }

        private void TruncateLog(SQLiteAsyncConnection sqlConnection)
        {
            sqlConnection.ExecuteAsync($"DELETE FROM {_tableName}")
                .ConfigureAwait(false);
        }


        protected override async Task<bool> WriteLogEventAsync(ICollection<LogEvent> logEventsBatch)
        {
            if ((logEventsBatch == null) || (logEventsBatch.Count == 0))
                return true;
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                var sqlConnection = GetSqLiteAsyncConnection();

                try
                {
                    await WriteToDatabaseAsync(logEventsBatch, sqlConnection).ConfigureAwait(false);
                    return true;
                }
                catch (SQLiteException e)
                {
                    SelfLog.WriteLine(e.Message);

                    if (e.Result != SQLite3.Result.Full)
                    {
                        return false;
                    }

                    if (!_rollOver)
                    {
                        SelfLog.WriteLine("Discarding log excessive of max database");
                        return true;
                    }

                    var dbExtension = Path.GetExtension(_databasePath);

                    var newFilePath = Path.Combine(Path.GetDirectoryName(_databasePath) ?? "Logs",
                        $"{Path.GetFileNameWithoutExtension(_databasePath)}-{DateTime.Now:yyyyMMdd_hhmmss.ff}{dbExtension}");

                    File.Copy(_databasePath, newFilePath, true);

                    TruncateLog(sqlConnection);
                    await WriteToDatabaseAsync(logEventsBatch, sqlConnection).ConfigureAwait(false);

                    SelfLog.WriteLine($"Rolling database to {newFilePath}");
                    return true;
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine(e.Message);
                    return false;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task WriteToDatabaseAsync(ICollection<LogEvent> logEventsBatch,
            SQLiteAsyncConnection sqlConnection)
        {
            var sqlInsertText = "INSERT INTO {0} (Timestamp, Level, Exception, RenderedMessage, Properties)";
            sqlInsertText += " VALUES (@timeStamp, @level, @exception, @renderedMessage, @properties)";
            sqlInsertText = string.Format(sqlInsertText, _tableName);

            foreach (var logEvent in logEventsBatch)
            {
                var logTimeStamp = _storeTimestampInUtc
                    ? logEvent.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff")
                    : logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                var logLevel = logEvent.Level.ToString();
                var exception = logEvent.Exception?.ToString() ?? string.Empty;
                var message = logEvent.MessageTemplate.Text;
                var properties = logEvent.Properties.Count > 0 ? logEvent.Properties.Json() : string.Empty;

                await sqlConnection.ExecuteAsync(sqlInsertText, new object[]
                {
                logTimeStamp,
                logLevel,
                exception,
                message,
                properties
                }).ConfigureAwait(false);
            }
        }
    }
}
