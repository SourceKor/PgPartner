using Npgsql;
using NpgsqlTypes;
using System;
using System.Threading.Tasks;

namespace PgPartner.SampleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var conn = new NpgsqlConnection("Server=localhost; Port=5432; Database=test; Username=postgres; Password=admin;");
            await conn.OpenAsync();

            var sampleApp = new SampleApp();
            sampleApp.CreateSampleTable(conn);

            var samples = sampleApp.GetSamples();

            await conn.BulkAddAsync(
                samples,
                (mapper, sample) => {
                    mapper.Map("id", sample.Id, NpgsqlDbType.Uuid);
                    mapper.Map("name", sample.Name, NpgsqlDbType.Text);
                    mapper.Map("amount", sample.ItemAmount, NpgsqlDbType.Numeric);
                    mapper.Map("sum", sample.ItemSum, NpgsqlDbType.Integer);
                    mapper.Map("created", sample.Created, NpgsqlDbType.Timestamp);
                    mapper.Map("created_date", sample.CreatedDate, NpgsqlDbType.TimestampTz);
                    mapper.Map("test1", sample.Test1, NpgsqlDbType.Bytea);
                    mapper.Map("test2", sample.Test2, NpgsqlDbType.Boolean);
                    mapper.Map("test3", sample.Test3, NpgsqlDbType.Inet);
                },
                sampleApp.Schema,
                sampleApp.Table
            );

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
