using NpgsqlTypes;

namespace PgPartner
{
    public interface IPgpMapper
    {
        void Map<T>(string columnName, T value, NpgsqlDbType dbType);
    }
}
