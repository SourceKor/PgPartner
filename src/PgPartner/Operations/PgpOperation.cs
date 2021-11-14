using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PgPartner.Operations
{
    internal class PgpOperation : OperationBase
    {
        private const string TableDetailsQuery = @"
            select column_name
	               ,data_type
	               ,is_nullable
	               ,column_default
	               ,character_maximum_length
	               ,numeric_precision
	               ,datetime_precision
	               ,interval_precision
            from information_schema.columns
            where table_schema = @schema
            and table_name = @table
            order by ordinal_position";

        public void CreateMirrorTempTable
            (
                NpgsqlConnection connection,
                string schemaName,
                string tableName
            )
        {
            var tempTable = tableName.Contains("\"")
                ? $"\"tmp_{tableName.Replace("\"", string.Empty)}\""
                : $"tmp_{tableName}";



            var fullTempTableName = GetFullTableName(schemaName, tempTable);

            using var command = connection.CreateCommand();
        }

        private async Task GetTableDetails(NpgsqlConnection connection, string schemaName, string tableName)
        {
            using var command = new NpgsqlCommand(TableDetailsQuery, connection);
            command.Parameters.AddWithValue("schema", NpgsqlDbType.Varchar, schemaName);
            command.Parameters.AddWithValue("table", NpgsqlDbType.Varchar, tableName);

            var reader = await command.ExecuteReaderAsync();
            var results = new List<object[]>();

            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values[i] = reader[i];
                }
                results.Add(values);
            }
        }
    }
}
