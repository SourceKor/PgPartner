# PgPartner (PostgreSQL Partner)

This .NET library extends [Npgsql](https://www.npgsql.org/) functions to simplify certain PostgreSQL tasks, including:
 - [BulkAdd](#bulkadd-npgsqlconnection-extension) (using PostgreSQL COPY function)

## BulkAdd (NpgsqlConnection extension)
This command utilizes the PostgreSQL COPY function to perform an optimized insert of .NET objects into a PostgreSQL database table of your choice.

### Example (sample .NET app can be found in src directory)
Assume you have a table such as:

**public."Samples"**
| Column Name | Data Type |
| --- | --- |
| id | uuid |
| name | varchar |
| sum | int |
| amount | decimal |

Using BulkAdd you can insert all domain model objects like so:

```csharp
// Domain Model Objects
var samples = new List<Sample>()
{
    new Sample { Id = Guid.NewGuid(), Name = "Test", ItemSum = 200, ItemAmount = 10 },
    new Sample { Id = Guid.NewGuid(), Name = "Test 2", ItemSum = 400, ItemAmount = 20 },
    new Sample { Id = Guid.NewGuid(), Name = "Test 3", ItemSum = 800, ItemAmount = 30 },
    new Sample { Id = Guid.NewGuid(), Name = "Test 4", ItemSum = 1200, ItemAmount = 40 },
    new Sample { Id = Guid.NewGuid(), Name = "Test 5", ItemSum = 2400, ItemAmount = 50 }
};

// Instantiate and open PostgreSQL database connection
using var conn = new NpgsqlConnection("<connection string>");
conn.Open();

// Execute BulkAdd by passing objects to insert into table, single object mapping, database schema, and database table
conn.BulkAdd(
    samples,
    (mapper, sample) => {
        mapper.Map("id", sample.Id, NpgsqlDbType.Uuid);
        mapper.Map("name", sample.Name, NpgsqlDbType.Text);
        mapper.Map("amount", sample.ItemAmount, NpgsqlDbType.Numeric);
        mapper.Map("sum", sample.ItemSum, NpgsqlDbType.Integer);
    },
    "public",
    "\"Samples\""
);
```

### Notes
- To specify quote wrapped schema, table, or columns, simply pass the quotes in directly (see how this is done for "Samples" table in example above)
- Make sure to correctly specify NpgsqlDbType