using Npgsql;
using System;
using System.Threading.Tasks;

namespace PgPartner.Operations
{
    internal class PgpOperation : OperationBase
    {
        private const string CreateTempTableQuery = @"
            create temporary table {0}
            select * from {1}";

        private const string TempTableExistsQuery = @"
            select *
            from information_schema.tables
            where table_name = @tableName";

        public async Task CreateMirrorTempTableAsync
            (
                NpgsqlConnection connection,
                string schemaName,
                string tableName,
                bool throwExceptionOnExists = true
            )
        {
            var tempTable = tableName.Contains("\"")
                ? $"\"tmp_{tableName.Replace("\"", string.Empty)}\""
                : $"tmp_{tableName}";

            using var queryCommand = new NpgsqlCommand(TempTableExistsQuery, connection);
            queryCommand.Parameters.AddWithValue("tableName", tempTable);
            var tempTableExistsResult = await queryCommand.ExecuteScalarAsync();

            if (tempTableExistsResult != null && throwExceptionOnExists)
            {
                throw new Exception($"Attempted to create {tempTable}, however this table already exists");
            }

            var cloneCommandText = string.Format(CreateTempTableQuery, tempTable, GetFullTableName(schemaName, tableName));

            using var cloneCommand = new NpgsqlCommand(cloneCommandText, connection);
            await cloneCommand.ExecuteNonQueryAsync();
        }
    }
}
