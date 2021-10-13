using Npgsql;
using PgPartner.SampleApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PgPartner.SampleApp
{
    public class SampleApp
    {
        private static string CreateSamplesTable = @"
            create extension if not exists ""uuid-ossp"";

            create table public.""Samples"" (
                id              uuid default uuid_generate_v4(),
                name            varchar(200) not null,
                sum             int null,
                amount          decimal null,
                created         timestamp null,
                created_date    timestamp with time zone null,
                test1           bytea null,
                test2           boolean default false,
                test3           inet null
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
            File.ReadAllLines(Path.GetFullPath("./MockData/MOCK_DATA.csv"))
                    .Skip(1)
                    .Select(s =>
                    {
                        var values = s.Split(',');

                        return new Sample
                        {
                            Id = new Guid(values[0]),
                            Name = values[1],
                            ItemSum = Convert.ToInt32(string.IsNullOrWhiteSpace(values[2]) ? null : values[2]),
                            ItemAmount = Convert.ToDecimal(string.IsNullOrWhiteSpace(values[3]) ? null : values[3]),
                            Created = string.IsNullOrWhiteSpace(values[4]) ? (DateTime?)null : DateTime.Parse(values[4]),
                            CreatedDate = string.IsNullOrWhiteSpace(values[5]) ? (DateTimeOffset?)null : DateTimeOffset.Parse(values[5]),
                            Test1 = Encoding.UTF8.GetBytes(values[6]),
                            Test2 = string.IsNullOrWhiteSpace(values[7]) ? false : bool.Parse(values[7]),
                            Test3 = string.IsNullOrWhiteSpace(values[8]) ? null : IPAddress.Parse(values[8])
                        };
                    })
                    .ToList(); // Ensures we're returning all results at once and not defering until enumeration is necessary
    }
}
