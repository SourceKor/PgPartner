namespace PgPartner.Options
{
    public abstract class OptionsBase
    {
        public bool QuoteTableName { get; set; } = false;

        public bool QuoteColumnNames { get; set; } = false;
    }
}
