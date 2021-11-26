using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using PgPartner.Test.Models;
using PgPartner.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgPartner.Test
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class BulkAddIntegrationTests : PostgresIntegrationTest
    {
        private const string CreateTestItemsTable = @"
            create extension if not exists ""uuid-ossp"";

            create table public.""TestItems"" (
                id              uuid default uuid_generate_v4(),
                text            varchar(200) not null,
                number          int null
            );";

        private const string DropTestItemsTable = @"drop table public.""TestItems"";";

        protected override void PostSetup()
        {
            var createTableCommand = new NpgsqlCommand(CreateTestItemsTable, Connection);
            createTableCommand.ExecuteNonQuery();
        }

        [TestMethod]
        public void BulkAdd()
        {
            // Arrange
            var expectedTestItems = GetTestItems();

            // Act
            Connection.BulkAdd(
                expectedTestItems,
                (mapper, entity) => {
                    mapper.Map("id", entity.Id, NpgsqlDbType.Uuid);
                    mapper.Map("text", entity.Text, NpgsqlDbType.Varchar);
                    mapper.Map("number", entity.Number, NpgsqlDbType.Integer);
                }, "public", "\"TestItems\"");

            var actualTestItems = QueryTestItems();

            // Assert
            actualTestItems.Should().HaveCount(4);

            foreach (var expectedItem in expectedTestItems)
            {
                var actualResult = actualTestItems.FirstOrDefault(a => a.Id == expectedItem.Id);
                actualResult.Should().NotBeNull();

                actualResult.Text.Should().Be(expectedItem.Text);
                actualResult.Number.Should().Be(expectedItem.Number);
            }
        }

        [TestMethod]
        public async Task BulkAddAsync()
        {
            // Arrange
            var expectedTestItems = GetTestItems();

            // Act
            await Connection.BulkAddAsync(
                expectedTestItems,
                (mapper, entity) => {
                    mapper.Map("id", entity.Id, NpgsqlDbType.Uuid);
                    mapper.Map("text", entity.Text, NpgsqlDbType.Varchar);
                    mapper.Map("number", entity.Number, NpgsqlDbType.Integer);
                }, "public", "\"TestItems\"");

            var actualTestItems = QueryTestItems();

            // Assert
            actualTestItems.Should().HaveCount(4);

            foreach (var expectedItem in expectedTestItems)
            {
                var actualResult = actualTestItems.FirstOrDefault(a => a.Id == expectedItem.Id);
                actualResult.Should().NotBeNull();

                actualResult.Text.Should().Be(expectedItem.Text);
                actualResult.Number.Should().Be(expectedItem.Number);
            }
        }

        [TestCleanup]
        public new void TearDown()
        {
            using var dropTableCommand = new NpgsqlCommand(DropTestItemsTable, Connection);
            dropTableCommand.ExecuteNonQuery();

            base.TearDown();
        }

        private IEnumerable<TestItem> GetTestItems() =>
            new List<TestItem>
            {
                new TestItem { Id = Guid.NewGuid(), Text = "Test", Number = 10 },
                new TestItem { Id = Guid.NewGuid(), Text = "Test2", Number = 20 },
                new TestItem { Id = Guid.NewGuid(), Text = "Test3", Number = 30 },
                new TestItem { Id = Guid.NewGuid(), Text = "Test4", Number = 40 }
            };

        private IEnumerable<TestItem> QueryTestItems() =>
            Connection.Query<TestItem>("select * from public.\"TestItems\"");
    }
}
