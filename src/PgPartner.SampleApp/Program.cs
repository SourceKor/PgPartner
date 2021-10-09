using Npgsql;
using NpgsqlTypes;
using System;

namespace PgPartner.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using var conn = new NpgsqlConnection("Server=localhost; Port=5432; Database=test; Username=postgres; Password=admin;");
            conn.Open();

            var sampleApp = new SampleApp();
            sampleApp.CreateSampleTable(conn);

            var samples = sampleApp.GetSamples();

            conn.BulkAdd(
                samples,
                (mapper, entity) => {
                    mapper.Map("id", entity.Id, NpgsqlDbType.Uuid);
                    mapper.Map("name", entity.Name, NpgsqlDbType.Varchar);
                    mapper.Map("amount", entity.ItemAmount, NpgsqlDbType.Integer);
                    mapper.Map("sum", entity.ItemSum, NpgsqlDbType.Numeric);
                },
                sampleApp.Schema,
                sampleApp.Table
            );

            Console.ReadKey();
        }
    }
}
