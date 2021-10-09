using Npgsql;
using NpgsqlTypes;
using PgPartner.Options;
using PgPartner.Options.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgPartner
{
    public static class PgPartner
    {
        private const string CopyCommand = "copy {0} ({1}) from stdin binary";

        /// <summary>
        /// Adds entities to a target table defined by schemaName and tableName
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fullTableName">
        ///     Full database table name. Include double quotes for schema/table names where
        ///     applicable (ex. public."Test", "Test"."HelloWorld", test.sample)
        /// </param>
        public static void BulkAdd<TEntity>
            (
                this NpgsqlConnection connection,
                IEnumerable<TEntity> entities,
                Action<IPgpMapper, TEntity> mapEntity,
                string schemaName,
                string tableName,
                PgPartnerAddOptions copyOption = null
            )
            where TEntity : class
        {
            var options = copyOption ?? new PgPartnerAddOptions();
            var fullTableName = OptionsParser.ParseTableName(schemaName, tableName, options.QuoteTableName);

            var (columnNames, columnValuesCollection) = GetMappingContext(entities, mapEntity);

            var copyColumns = columnNames.Aggregate((x, y) => $"{x},{y}");

            var command = string.Format(CopyCommand, fullTableName, copyColumns);
            using var writer = connection.BeginBinaryImport(command);

            foreach (var columnValues in columnValuesCollection)
            {
                writer.StartRow();

                foreach (var (value, dbType) in columnValues)
                {
                    if (value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        writer.Write(value, dbType);
                    }
                }

                writer.Complete();
                writer.Close();
            }
        }

        private static (IEnumerable<string>, List<List<(object, NpgsqlDbType)>>) GetMappingContext<TEntity>
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
    }
}
