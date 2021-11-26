using Npgsql;
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
        /// Creates a temp table that mirrors the source table that's passed in
        /// </summary>
        /// <param name="schemaName">Source schema of the table to mirror</param>
        /// <param name="tableName">Source table to mirror</param>
        /// <param name="throwExceptionOnExists">Throws an exception if the temp table to be created already exists [default = true]</param>
        public static void CreateMirrorTempTable
            (
                this NpgsqlConnection connection,
                string schemaName,
                string tableName,
                bool throwExceptionOnExists = true
            )
        {
            var operation = new PgpOperation();
            operation.CreateMirrorTempTableAsync(connection, schemaName, tableName, throwExceptionOnExists)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Creates a temp table that mirrors the source table that's passed in
        /// </summary>
        /// <param name="schemaName">Source schema of the table to mirror</param>
        /// <param name="tableName">Source table to mirror</param>
        /// <param name="throwExceptionOnExists">Throws an exception if the temp table to be created already exists [default = true]</param>
        public static async Task CreateMirrorTempTableAsync
            (
                this NpgsqlConnection connection,
                string schemaName,
                string tableName,
                bool throwExceptionOnExists = true
            )
        {
            var operation = new PgpOperation();
            await operation.CreateMirrorTempTableAsync(connection, schemaName, tableName, throwExceptionOnExists);
        }
    }
}
