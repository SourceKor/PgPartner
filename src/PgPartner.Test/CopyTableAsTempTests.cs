using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using PgPartner.Test.Utilities;
using System.Threading.Tasks;

namespace PgPartner.Test
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class CopyTableAsTempTests : PostgresIntegrationTest
    {
        private const string CreateSamplesTable = @"
            create extension if not exists ""uuid-ossp"";

            create table public.""SamplesTest"" (
                id              int,
                text            varchar(200) not null
            );";

        private const string AddSamplesCommand = @"
            insert into public.""SamplesTest"" (id, text)
            values (1, 'Hello');
            
            insert into public.""SamplesTest"" (id, text)
            values (2, 'World');";

        private static string DropSamplesTable = @"drop table public.""SamplesTest"";";

        protected override void PostSetup()
        {
            using var createTableCommand = new NpgsqlCommand(CreateSamplesTable, Connection);
            createTableCommand.ExecuteNonQuery();
        }

        [TestMethod]
        public void CopyTableAsTemp()
        {
            // Arrange
            AddSampleItems();

            // Act
            var details = Connection.CopyTableAsTemp("public", "\"SamplesTest\"");
            var itemCount = GetTempTableItemCount(details.SchemaName, details.TableName);

            // Assert
            itemCount.Should().Be(0);

            using var dropTableCommand = new NpgsqlCommand($"drop table {details.SchemaName}.{details.TableName}", Connection);
            dropTableCommand.ExecuteNonQuery();
        }

        [TestMethod]
        public async Task CopyTableAsTempAsync()
        {
            // Arrange
            var tempTableName = "temp_samplestest";
            AddSampleItems();

            // Act
            var details = await Connection.CopyTableAsTempAsync("public", "\"SamplesTest\"", tempTableName);
            var itemCount = GetTempTableItemCount(details.SchemaName, tempTableName);

            // Assert
            itemCount.Should().Be(0);

            using var dropTableCommand = new NpgsqlCommand($"drop table {details.SchemaName}.{tempTableName}", Connection);
            dropTableCommand.ExecuteNonQuery();
        }

        [TestCleanup]
        public new void TearDown()
        {
            using var dropTableCommand = new NpgsqlCommand(DropSamplesTable, Connection);
            dropTableCommand.ExecuteNonQuery();

            base.TearDown();
        }

        private void AddSampleItems()
        {
            using var addCommand = new NpgsqlCommand(AddSamplesCommand, Connection);
            addCommand.ExecuteNonQuery();
        }

        private int GetTempTableItemCount(string tempSchemaName, string tempTableName)
        {
            using var queryCommand = new NpgsqlCommand($"select count(*) from {tempSchemaName}.{tempTableName}", Connection);
            return int.Parse(queryCommand.ExecuteScalar().ToString());
        }
    }
}
