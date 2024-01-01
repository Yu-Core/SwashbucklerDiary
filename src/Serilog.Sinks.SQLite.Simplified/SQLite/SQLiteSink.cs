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
using Serilog.Events;
using Serilog.Sinks.Extensions;
using SQLite;

//There are changes here. Code source https://github.com/saleem-mirza/serilog-sinks-sqlite/issues/24#issuecomment-608555305
namespace Serilog.Sinks.SQLite
{
    internal class SQLiteSink : ILogEventSink
    {
        private readonly string _databasePath; 

        private readonly bool _storeTimestampInUtc; 

        private readonly bool _rollOver; 

        private readonly string _tableName;

        private readonly int _batchSize;

        private const int MaxSupportedBatchSize = 1_000;

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public SQLiteSink(
            string sqlLiteDbPath,
            string tableName,
            bool storeTimestampInUtc,
            uint batchSize = 100,
            bool rollOver = true) 
        {
            _databasePath = sqlLiteDbPath;
            _tableName = tableName;
            _storeTimestampInUtc = storeTimestampInUtc;
            _rollOver = rollOver;
            _batchSize = Math.Min(Math.Max((int)batchSize, 1), MaxSupportedBatchSize);

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
            using var conn = GetSqLiteAsyncConnection();
            CreateSqlTable(conn);
        }

        private SQLiteConnection GetSqLiteAsyncConnection()
        {
            var sqlConString = new SQLiteConnectionString(_databasePath, true);
            var sqLiteConnection = new SQLiteConnection(sqlConString);
            return sqLiteConnection;
        }

        private void CreateSqlTable(SQLiteConnection sqlConnection)
        {
            var colDefs = "id INTEGER PRIMARY KEY AUTOINCREMENT,";
            colDefs += "Timestamp TEXT,";
            colDefs += "Level VARCHAR(10),";
            colDefs += "Exception TEXT,";
            colDefs += "RenderedMessage TEXT,";
            colDefs += "Properties TEXT";

            var sqlCreateText = $"CREATE TABLE IF NOT EXISTS {_tableName} ({colDefs})";

            sqlConnection.Execute(sqlCreateText);
        }

        private void PushEvent(LogEvent logEvent)
        {
            using var sqlConnection = GetSqLiteAsyncConnection();
            WriteToDatabase(logEvent, sqlConnection);
        }

        private void WriteToDatabase(LogEvent logEvent,
            SQLiteConnection sqlConnection)
        {
            var sqlInsertText = "INSERT INTO {0} (Timestamp, Level, Exception, RenderedMessage, Properties)";
            sqlInsertText += " VALUES (@timeStamp, @level, @exception, @renderedMessage, @properties)";
            sqlInsertText = string.Format(sqlInsertText, _tableName);

            var logTimeStamp = _storeTimestampInUtc
                    ? logEvent.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff")
                    : logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            var logLevel = logEvent.Level.ToString();
            var exception = logEvent.Exception?.ToString() ?? string.Empty;
            var message = logEvent.MessageTemplate.Text;
            var properties = logEvent.Properties.Count > 0 ? logEvent.Properties.Json() : string.Empty;

            sqlConnection.Execute(sqlInsertText, new object[]
            {
                logTimeStamp,
                logLevel,
                exception,
                message,
                properties
            });
        }
    }
}
