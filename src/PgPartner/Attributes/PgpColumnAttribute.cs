using System;

namespace PgPartner.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PgpColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public PgpColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
