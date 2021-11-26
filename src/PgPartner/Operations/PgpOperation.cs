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
            where lower(table_name) = lower(@tableName)";

        public async Task<TableDetails> CopyTableAsTempAsync
            (
                NpgsqlConnection connection,
                string schemaName,
                string tableName,
                string tempTableName = null,
                OnCommitOptions commitOptions = OnCommitOptions.PreserveRows
            )
        {
            var tempTable = string.IsNullOrWhiteSpace(tempTableName)
                ? $"tmp_{tableName.Replace("\"", string.Empty)}"
                : tempTableName;

            var existingTempTableDetails = await GetTempTableDetails(connection, tempTable);

            if (existingTempTableDetails != null)
            {
                return existingTempTableDetails;
            }

            var options = commitOptions switch
            {
                OnCommitOptions.PreserveRows => "on commit preserve rows",
                OnCommitOptions.DeleteRows => "on commit delete rows",
                OnCommitOptions.Drop => "on commit drop",
                _ => throw new ArgumentException($"Invalid {nameof(commitOptions)}")
            };

            var cloneCommandText = string.Format(CreateTempTableQuery, $"{tempTable} {options}", GetFullTableName(schemaName, tableName));

            using var cloneCommand = new NpgsqlCommand(cloneCommandText, connection);
            await cloneCommand.ExecuteNonQueryAsync();

            return await GetTempTableDetails(connection, tempTable);
        }

        private async Task<TableDetails> GetTempTableDetails(NpgsqlConnection connection, string tempTableName)
        {
            using var queryCommand = new NpgsqlCommand(TempTableExistsQuery, connection);
            queryCommand.Parameters.AddWithValue("tableName", tempTableName);
            
            using var reader = await queryCommand.ExecuteReaderAsync();
            TableDetails result = null;

            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
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
