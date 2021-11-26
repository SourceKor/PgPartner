using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace PgPartner.Test
{
    [TestClass]
    public abstract class PostgresIntegrationTest
    {
        protected NpgsqlConnection Connection;

        [TestInitialize]
        public void SetUp()
        {
            Connection = new NpgsqlConnection("Server=localhost; Port=5432; Database=test; Username=postgres; Password=admin;");
            Connection.Open();

            PostSetup();
        }

        /// <summary>
        /// Executes right after a connection has been established and opened
        /// </summary>
        protected virtual void PostSetup()
        {
            // Do nothing for initial implementation
        }

        [TestCleanup]
        public void TearDown()
        {
            Connection.Dispose();
        }
    }
}
