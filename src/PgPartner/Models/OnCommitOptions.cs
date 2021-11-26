namespace PgPartner.Models
{
    /// <summary>
    /// The behavior that can be controlled at the end of a transaction block.
    /// </summary>
    public enum OnCommitOptions
    {
        /// <summary>
        /// No special action is taken at the ends of transactions. This is the default behavior.
        /// </summary>
        PreserveRows = 0,
        /// <summary>
        /// All rows in the temporary table will be deleted at the end of each transaction block. Essentially, an automatic TRUNCATE is done at each commit.
        /// </summary>
        DeleteRows = 1,
        /// <summary>
        /// The temporary table will be dropped at the end of the current transaction block.
        /// </summary>
        Drop = 2
    }
}
