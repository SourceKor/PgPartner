using Npgsql;
using NpgsqlTypes;
using System;

namespace PgPartner
{
    public class PgpMapper
    {
        private readonly NpgsqlBinaryImporter _importer;

        public PgpMapper(NpgsqlBinaryImporter importer)
        {
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
        }

        public void Map<T>(T value, NpgsqlDbType dbType)
        {
            if (value == null)
            {
                _importer.WriteNull();
            }
            else
            {
                _importer.Write(value, dbType);
            }
        }
    }
}
