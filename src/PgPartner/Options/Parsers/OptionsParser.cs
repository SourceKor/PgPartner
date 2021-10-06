namespace PgPartner.Options.Parsers
{
    public static class OptionsParser
    {
        public static string ParseTableName(string schema, string table, bool quoteWrap) =>
            $"{ParseName(schema, quoteWrap)}.{ParseName(table, quoteWrap)}";

        public static string ParseName(string name, bool quoteWrap) =>
            quoteWrap ? $"\"{name}\"" : name;
    }
}
