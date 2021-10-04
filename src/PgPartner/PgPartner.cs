using Npgsql;
using System.Collections.Generic;

namespace PgPartner
{
    public static class PgPartner
    {
        private const string CopyCommand = "copy {0} from stdin binary";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fullTableName">
        ///     Full database table name. Include double quotes for schema/table names where
        ///     applicable (ex. public."Test", "Test"."HelloWorld", test.sample)
        /// </param>
        public static void Copy<TEntity>(this NpgsqlConnection connection, IEnumerable<TEntity> entities, string fullTableName)
            where TEntity : class
        {
            var command = string.Format(CopyCommand, fullTableName);
            using var writer = connection.BeginBinaryImport(command);

            var entityType = typeof(TEntity).Cus;
        }
    }
}
