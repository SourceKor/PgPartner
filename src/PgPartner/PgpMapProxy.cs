using NpgsqlTypes;
using System.Collections.Generic;

namespace PgPartner
{
    /// <summary>
    /// Proxies map context to the consumer
    /// </summary>
    public class PgpMapProxy : IPgpMapper
    {
        private bool _collectColumnNames = true;

        public IEnumerable<string> ColumnNames { get; private set; } = new List<string>();

        public IEnumerable<(object, NpgsqlDbType)> ColumnValues { get; private set; } = new List<(object, NpgsqlDbType)>();

        public void DisableColumnNameCollect()
        {
            if (_collectColumnNames == true)
            {
                _collectColumnNames = false;
            }
        }

        public void ClearColumnValueCollection() => ColumnValues = new List<(object, NpgsqlDbType)>();

        public void Map<T>(string columnName, T value, NpgsqlDbType dbType)
        {
            // We only want to make sure to collect column names once. Object consumer should decide when it's time to disable.
            if (_collectColumnNames)
            {
                var existingColumnNames = new List<string>(ColumnNames)
                {
                    columnName
                };
                ColumnNames = existingColumnNames;
            }

            var existingColumnValues = new List<(object, NpgsqlDbType)>(ColumnValues)
            {
                (value, dbType)
            };
            ColumnValues = existingColumnValues;
        }
    }
}
