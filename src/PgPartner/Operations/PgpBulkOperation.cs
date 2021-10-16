using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgPartner.Operations
{
    internal class PgpBulkOperation : OperationBase
    {
        private const string CopyCommand = "copy {0} ({1}) from stdin binary";

        public async Task BulkAddAsync<TEntity>
            (
                NpgsqlConnection connection,
                IEnumerable<TEntity> entities,
                Action<IPgpMapper, TEntity> mapEntity,
                string schemaName,
                string tableName
            )
            where TEntity : class
        {
            var (columnNames, columnValuesCollection) = BuildMappingContext(entities, mapEntity);
            var command = BuildCopyCommand(schemaName, tableName, columnNames);

            using var writer = connection.BeginBinaryImport(command);

            foreach (var columnValues in columnValuesCollection)
            {
                await writer.StartRowAsync();

                foreach (var (value, dbType) in columnValues)
                {
                    if (value == null)
                    {
                        await writer.WriteNullAsync();
                    }
                    else
                    {
                        await writer.WriteAsync(value, dbType);
                    }
                }
            }

            try
            {
                await writer.CompleteAsync();
                await writer.CloseAsync();
            }
            catch (PostgresException pex)
            {
                HandlePostgresException(pex);
                throw;
            }
            finally
            {
                await writer.DisposeAsync();
            }
        }

        private string BuildCopyCommand(string schemaName, string tableName, IEnumerable<string> columnNames)
        {
            var fullTableName = GetFullTableName(schemaName, tableName);
            var copyColumns = columnNames.Aggregate((x, y) => $"{x},{y}");
            return string.Format(CopyCommand, fullTableName, copyColumns);
        }

        private static (IEnumerable<string>, List<List<(object, NpgsqlDbType)>>) BuildMappingContext<TEntity>
            (
                IEnumerable<TEntity> entities,
                Action<IPgpMapper, TEntity> mapEntity
            )
            where TEntity : class
        {
            var mapper = new PgpMapProxy();

            var columnValuesCollection = new List<List<(object, NpgsqlDbType)>>();

            foreach (var entity in entities)
            {
                mapEntity(mapper, entity);

                mapper.DisableColumnNameCollect();

                var columnValuesCopy = new List<(object, NpgsqlDbType)>(mapper.ColumnValues);
                columnValuesCollection.Add(columnValuesCopy);

                mapper.ClearColumnValueCollection();
            }

            return (mapper.ColumnNames, columnValuesCollection);
        }

        private void HandlePostgresException(PostgresException ex)
        {
            if (ex.Message == "08P01: insufficient data left in message")
            {
                throw new Exception($"Postgres error most likely caused by invalid .NET to NpgsqlDbType mapping. Postgres Exception: {ex.Message}");
            }
        }
    }
}
