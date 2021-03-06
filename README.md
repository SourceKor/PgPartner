[![Build Status](https://dev.azure.com/okrysko/PgPartner/_apis/build/status/SourceKor.PgPartner?branchName=main)](https://dev.azure.com/okrysko/PgPartner/_build/latest?definitionId=4&branchName=main)

# PgPartner (PostgreSQL Partner)

This .NET library extends [Npgsql](https://www.npgsql.org/) functions to simplify certain PostgreSQL tasks, including:
 - [BulkAdd](#bulkadd-npgsqlconnection-extension) (using PostgreSQL COPY function)
 - [BulkAddAsync](#bulkadd-npgsqlconnection-extension) (using PostgreSQL COPY function)
 - [CopyTableAsTemp](#copytableastemp-npgsqlconnection-extension) - Creates an empty temporary table that mirrors an existing table
 - [CopyTableAsTempAsync](#copytableastemp-npgsqlconnection-extension) - Creates an empty temporary table that mirrors an existing table

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

### Sync

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

### Async
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
await conn.OpenAsync();

// Execute BulkAdd by passing objects to insert into table, single object mapping, database schema, and database table
await conn.BulkAddAsync(
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
- Make sure to correctly specify association of NpgsqlDbType to .NET CLR type, otherwise you will see various errors.

## CopyTableAsTemp (NpgsqlConnection extension)
This command will create a temporary table that mirrors an existing table. This is useful for scenarios where you might need to synchronize old and new data for any table. In such a scenario you can leverage this command to create a temporary table that mirrors your existing table, use BulkAdd to add all records to the temporary table and then execute sync operations such as upsert/delete/etc.

### Example (sample .NET app can be found in src directory)
Assume you have a table such as:

**public."Samples"**
| Column Name | Data Type |
| --- | --- |
| id | uuid |
| name | varchar |
| sum | int |
| amount | decimal |

Using CopyTableAsTemp you can mirror this table as a new temporary table like:

### Sync
```c#
// Instantiate and open PostgreSQL database connection
using var conn = new NpgsqlConnection("<connection string>");
conn.Open();

// Execute a copy table as temporary table operation
var tableDetails = conn.CopyTableAsTemp("public", "\"Samples\"");

// At this point a new table with the name of "tmp_samples" will exist. The tableDetails variable will contain all details of the newly created table.
```

If you'd like to create a temp table with your own name, simply pass in the name you'd like to create the temporary table with, like so:

```c#
...
// Execute a copy table as temporary table operation
var tableDetails = conn.CopyTableAsTemp("public", "\"Samples\"", "temp_mytable");

// At this point a new table with the name of "temp_mytable" will exist. The tableDetails variable will contain all details of the newly created table.
```

### Async
```c#
// Instantiate and open PostgreSQL database connection
using var conn = new NpgsqlConnection("<connection string>");
conn.Open();

// Execute a copy table as temporary table operation
var tableDetails = await conn.CopyTableAsTempAsync("public", "\"Samples\"");

// At this point a new table with the name of "tmp_samples" will exist. The tableDetails variable will contain all details of the newly created table.
```