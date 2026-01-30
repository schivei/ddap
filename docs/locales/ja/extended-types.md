# Extended Types Support

DDAP now supports additional .NET types in both data providers and GraphQL:

## Supported Extended Types

### Unsigned Integer Types
- `uint` (32-bit unsigned integer)
- `ulong` (64-bit unsigned integer)
- `ushort` (16-bit unsigned integer)
- `sbyte` (8-bit signed integer)

### Modern Date/Time Types
- `DateOnly` - Date without time component
- `TimeOnly` - Time without date component
- `TimeSpan` - Duration/interval
- `DateTimeOffset` - Date and time with timezone offset

## Usage

### In Dapper Provider

The Dapper provider automatically maps SQL types to these CLR types:

**MySQL Unsigned Types:**
```sql
CREATE TABLE Users (
    Id INT UNSIGNED PRIMARY KEY,
    Credits BIGINT UNSIGNED,
    Age SMALLINT UNSIGNED,
    Status TINYINT UNSIGNED
);
```

Maps to:
```csharp
public class User {
    public uint Id { get; set; }
    public ulong Credits { get; set; }
    public ushort Age { get; set; }
    public byte Status { get; set; }
}
```

**Date/Time Types:**
```sql
CREATE TABLE Events (
    EventDate DATE,              -- Maps to DateOnly
    EventTime TIME,              -- Maps to TimeOnly
    Duration INTERVAL,           -- Maps to TimeSpan (PostgreSQL)
    CreatedAt DATETIMEOFFSET     -- Maps to DateTimeOffset
);
```

### In GraphQL

Enable extended types support in your GraphQL configuration:

```csharp
services.AddDdap(options => 
{
    options.ConnectionString = "...";
})
.AddGraphQL(graphql => 
{
    graphql.AddExtendedTypes(); // Enable support for extended types
});
```

This adds:
- Type converters for unsigned integers (maps to signed equivalents)
- Custom scalar types for `DateOnly` and `TimeOnly`
- Support for `TimeSpan` and `DateTimeOffset`

## GraphQL Schema

The extended types appear in your GraphQL schema as:

```graphql
scalar DateOnly  # ISO 8601 date: "2024-01-29"
scalar TimeOnly  # ISO 8601 time: "14:30:00"

type User {
    id: Int!           # uint mapped to Int
    credits: Long!     # ulong mapped to Long
    age: Short!        # ushort mapped to Short
    birthDate: DateOnly
    workStartTime: TimeOnly
}
```

## Breaking Changes

⚠️ **Important:** Starting with this version:
- SQL `date` type now maps to `DateOnly` (was `DateTime`)
- SQL `time` type now maps to `TimeOnly` (was `TimeSpan`)

If you need `TimeSpan`, use:
- PostgreSQL: `INTERVAL` type
- Other databases: Store as numeric (milliseconds/ticks)

## Example Query

```graphql
query {
    users {
        id
        credits
        birthDate
        workStartTime
    }
}
```

Response:
```json
{
    "data": {
        "users": [
            {
                "id": 1,
                "credits": 9223372036854775807,
                "birthDate": "1990-05-15",
                "workStartTime": "09:00:00"
            }
        ]
    }
}
```

## Notes

- Unsigned types are converted to their signed equivalents in GraphQL (which doesn't have unsigned types)
- Values exceeding signed type ranges will wrap around (unchecked conversion)
- Custom scalar types serialize to ISO 8601 strings for broad compatibility
