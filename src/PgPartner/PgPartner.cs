using Npgsql;
using PgPartner.Models;
using PgPartner.Operations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PgPartner
{
    public static class PgPartner
    {
        /// <summary>
        /// Asynchronously adds entities to a target table using Postgres COPY command
        /// </summary>
        /// <param name="entities">Entities to add into the destination table</param>
        /// <param name="mapEntity">Map action to execute for a single entity when entites are being added to the destination table</param>
        /// <param name="schemaName">Destination schema to add entities into. If schema name requires double quotes, add them as escaped quotes in the string being passed in.</param>
        /// <param name="tableName">Destination table to add entities into. If table name requires double quotes, add them as escaped quotes in the string being passed in.</param>
        public static async Task BulkAddAsync<TEntity>
            (
                this NpgsqlConnection connection,
                IEnumerable<TEntity> entities,
                Action<IPgpMapper, TEntity> mapEntity,
                string schemaName,
                string tableName
            )
            where TEntity : class
        {
            var bulkOperation = new PgpBulkOperation();
            await bulkOperation.BulkAddAsync(connection, entities, mapEntity, schemaName, tableName);
        }

        /// <summary>
        /// Adds entities to a target table using Postgres COPY command
        /// </summary>
        /// <param name="entities">Entities to add into the destination table</param>
        /// <param name="mapEntity">Map action to execute for a single entity when entites are being added to the destination table</param>
        /// <param name="schemaName">Destination schema to add entities into. If schema name requires double quotes, add them as escaped quotes in the string being passed in.</param>
        /// <param name="tableName">Destination table to add entities into. If table name requires double quotes, add them as escaped quotes in the string being passed in.</param>
        public static void BulkAdd<TEntity>
            (
                this NpgsqlConnection connection,
                IEnumerable<TEntity> entities,
                Action<IPgpMapper, TEntity> mapEntity,
                string schemaName,
                string tableName
            )
            where TEntity : class
        {
            var bulkOperation = new PgpBulkOperation();
            bulkOperation.BulkAddAsync(connection, entities, mapEntity, schemaName, tableName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Creates a temp table (with a default name of tmp_<tableName> if tempTableName is not passed) that mirrors the source table passed in
        /// </summary>
        /// <param name="schemaName">Source schema of the table to mirror</param>
        /// <param name="tableName">Source table to mirror</param>
        /// <param name="tempTableName">Temp table name to use, if nothing is passed the default will be tmp_<tableName></param>
        /// <returns>Temp table details</returns>
        public static TableDetails CopyTableAsTemp
            (
                this NpgsqlConnection connection,
                string schemaName,
                string tableName,
                string tempTableName = null
            )
        {
            var operation = new PgpOperation();
            return operation.CopyTableAsTempAsync(connection, schemaName, tableName, tempTableName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Creates a temp table (with a default name of tmp_<tableName> if tempTableName is not passed) that mirrors the source table passed in
        /// </summary>
        /// <param name="schemaName">Source schema of the table to mirror</param>
        /// <param name="tableName">Source table to mirror</param>
        /// <param name="tempTableName">Temp table name to use, if nothing is passed the default will be tmp_<tableName></param>
        /// <returns>Temp table details</returns>
        public static async Task<TableDetails> CopyTableAsTempAsync
            (
                this NpgsqlConnection connection,
                string schemaName,
                string tableName,
                string tempTableName = null
            )
        {
            var operation = new PgpOperation();
            return await operation.CopyTableAsTempAsync(connection, schemaName, tableName, tempTableName);
        }
    }
}
