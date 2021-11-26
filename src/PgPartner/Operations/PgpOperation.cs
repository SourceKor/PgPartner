using Npgsql;
using PgPartner.Models;
using System;
using System.Threading.Tasks;

namespace PgPartner.Operations
{
    internal class PgpOperation : OperationBase
    {
        private const string CreateTempTableQuery = @"
            create temporary table {0} as
            select * from {1}";

        private const string TempTableExistsQuery = @"
            select table_schema
                   ,table_name
                   ,table_type
            from information_schema.tables
            where table_name = @tableName";

        public async Task<TableDetails> CopyTableAsTempAsync
            (
                NpgsqlConnection connection,
                string schemaName,
                string tableName,
                bool throwExceptionOnExists = true
            )
        {
            var tempTable = $"tmp_{tableName.Replace("\"", string.Empty)}";

            if (throwExceptionOnExists)
            {
                var tempTableExistsResult = GetTempTableDetails(connection, tempTable);

                if (tempTableExistsResult != null)
                {
                    throw new Exception($"Attempted to create {tempTable}, however this table already exists");
                }
            }

            var cloneCommandText = string.Format(CreateTempTableQuery, tempTable, GetFullTableName(schemaName, tableName));

            using var cloneCommand = new NpgsqlCommand(cloneCommandText, connection);
            await cloneCommand.ExecuteNonQueryAsync();

            return await GetTempTableDetails(connection, tempTable);
        }

        private async Task<TableDetails> GetTempTableDetails(NpgsqlConnection connection, string tempTableName)
        {
            using var queryCommand = new NpgsqlCommand(TempTableExistsQuery, connection);
            queryCommand.Parameters.AddWithValue("tableName", tempTableName);
            
            var reader = await queryCommand.ExecuteReaderAsync();
            TableDetails result = null;

            while (await reader.ReadAsync())
            {
                if (reader[0] != null)
                {
                    result = new TableDetails
                    {
                        SchemaName = reader.GetString(0),
                        TableName = reader.GetString(1),
                        TableType = reader.GetString(2)
                    };
                }
            }

            return result;
        }
    }
}
