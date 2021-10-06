using Npgsql;
using PgPartner.Options;
using PgPartner.Options.Parsers;
using System;
using System.Collections.Generic;

namespace PgPartner
{
    public static class PgPartner
    {
        private const string CopyCommand = "copy {0} from stdin binary";

        /// <summary>
        /// Adds entities to a target table defined by schemaName and tableName
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fullTableName">
        ///     Full database table name. Include double quotes for schema/table names where
        ///     applicable (ex. public."Test", "Test"."HelloWorld", test.sample)
        /// </param>
        public static void Add<TEntity>
            (
                this NpgsqlConnection connection,
                IEnumerable<TEntity> entities,
                Action<PgpMapper, TEntity> mapEntity,
                string schemaName,
                string tableName,
                PgPartnerAddOptions copyOption = null
            )
            where TEntity : class
        {
            var options = copyOption ?? new PgPartnerAddOptions();
            var fullTableName = OptionsParser.ParseTableName(schemaName, tableName, options.QuoteTableName);

            var command = string.Format(CopyCommand, fullTableName);
            using var writer = connection.BeginBinaryImport(command);

            foreach (var entity in entities)
            {
                writer.StartRow();
                var mapper = new PgpMapper(writer);

                mapEntity(mapper, entity);

                writer.Close();
            }
        }
    }
}
