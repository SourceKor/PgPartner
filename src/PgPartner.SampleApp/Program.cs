using Npgsql;
using NpgsqlTypes;
using PgPartner.SampleApp.Models;
using System;
using System.Collections.Generic;

namespace PgPartner.SampleApp
{
    class Program
    {
        private static string CreateSamplesTable = @"
            create extension if not exists ""uuid-ossp"";

            create table public.samples (
                id          uuid default uuid_generate_v4(),
                name        varchar(200) not null,
                sum         int null,
                amount      decimal null
            );";

        private static string QueryForSamplesTable = @"
            select *
            from information_schema.tables
            where table_schema = 'public'
            and table_name = 'samples'";


        static void Main(string[] args)
        {
            using var conn = new NpgsqlConnection("Server=localhost; Port=5432; Database=test; Username=postgres; Password=admin;");
            conn.Open();

            var queryCommand = new NpgsqlCommand(QueryForSamplesTable, conn);
            var tableDoesNotExist = queryCommand.ExecuteScalar() == null;

            if (tableDoesNotExist)
            {
                var createSampleTableCommand = new NpgsqlCommand(CreateSamplesTable, conn);
                createSampleTableCommand.ExecuteNonQuery();
            }

            var samples = new List<Sample>()
            {
                new Sample { Id = Guid.NewGuid(), Name = "Test", ItemSum = 200, ItemAmount = 10 },
                new Sample { Id = Guid.NewGuid(), Name = "Test 2", ItemSum = 400, ItemAmount = 20 }
            };

            conn.Add(
                samples,
                (mapper, entity) => {
                    mapper.Map(entity.Id, NpgsqlDbType.Uuid);
                    mapper.Map(entity.Name, NpgsqlDbType.Varchar);
                    mapper.Map(entity.ItemAmount, NpgsqlDbType.Integer);
                    mapper.Map(entity.ItemSum, NpgsqlDbType.Numeric);
                },
                "public",
                "samples"
            );

            Console.ReadKey();
        }
    }
}
