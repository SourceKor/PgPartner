using Npgsql;
using PgPartner.SampleApp.Models;
using System;
using System.Collections.Generic;

namespace PgPartner.SampleApp
{
    public class SampleApp
    {
        private static string CreateSamplesTable = @"
            create extension if not exists ""uuid-ossp"";

            create table public.""Samples"" (
                id          uuid default uuid_generate_v4(),
                name        varchar(200) not null,
                sum         int null,
                amount      decimal null
            );";

        private static string QueryForSamplesTable = @"
            select *
            from information_schema.tables
            where table_schema = 'public'
            and table_name = 'Samples'";

        public string Schema => "public";
        public string Table => "\"Samples\"";

        /// <summary>
        /// Creates the sample table if it doesn't already exist
        /// </summary>
        /// <param name="conn"></param>
        public void CreateSampleTable(NpgsqlConnection conn)
        {
            var queryCommand = new NpgsqlCommand(QueryForSamplesTable, conn);
            var tableDoesNotExist = queryCommand.ExecuteScalar() == null;

            if (tableDoesNotExist)
            {
                var createSampleTableCommand = new NpgsqlCommand(CreateSamplesTable, conn);
                createSampleTableCommand.ExecuteNonQuery();
            }
        }

        public IEnumerable<Sample> GetSamples() =>
            new List<Sample>()
            {
                new Sample { Id = Guid.NewGuid(), Name = "Test", ItemSum = 200, ItemAmount = 10 },
                new Sample { Id = Guid.NewGuid(), Name = "Test 2", ItemSum = 400, ItemAmount = 20 },
                new Sample { Id = Guid.NewGuid(), Name = "Test 3", ItemSum = 800, ItemAmount = 30 },
                new Sample { Id = Guid.NewGuid(), Name = "Test 4", ItemSum = 1200, ItemAmount = 40 },
                new Sample { Id = Guid.NewGuid(), Name = "Test 5", ItemSum = 2400, ItemAmount = 50 }
            };
    }
}
