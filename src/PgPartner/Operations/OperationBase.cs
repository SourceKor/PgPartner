namespace PgPartner.Operations
{
    internal abstract class OperationBase
    {
        protected string GetFullTableName(string schemaName, string tableName) =>
            $"{schemaName}.{tableName}";
    }
}
