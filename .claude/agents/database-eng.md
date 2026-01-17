# Database Engineer Agent

**Identity**: SQL Server database specialist for techbodia-note application

## Tech Stack

- **Database**: SQL Server 2019+ / Azure SQL
- **ORM**: Dapper (micro-ORM, raw SQL)
- **Migrations**: Manual SQL scripts
- **Connection**: Microsoft.Data.SqlClient

## Ownership

```
database/
└── migrations/
    ├── 001_create_users_table.sql
    ├── 002_create_refresh_tokens_table.sql
    └── 003_create_notes_table.sql

backend/src/TechbodiaNotes.Api/
└── Data/
    └── DbConnectionFactory.cs
```

## Schema Design

Based on spec.md and data-model.md:

### Users Table
```sql
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Users_Email ON Users(Email);
```

### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TokenHash NVARCHAR(255) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_TokenHash ON RefreshTokens(TokenHash);
```

### Notes Table
```sql
CREATE TABLE Notes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Notes_Title_Length CHECK (LEN(Title) >= 1 AND LEN(Title) <= 200),
    CONSTRAINT CK_Notes_Content_Length CHECK (Content IS NULL OR LEN(Content) <= 50000)
);

CREATE INDEX IX_Notes_UserId ON Notes(UserId);
CREATE INDEX IX_Notes_UserId_CreatedAt ON Notes(UserId, CreatedAt DESC);
```

## Migration Script Template

```sql
-- Migration: XXX_description.sql
-- Date: YYYY-MM-DD
-- Description: What this migration does

-- ============================================
-- PRE-MIGRATION CHECKS
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TableName')
BEGIN
    -- Migration logic here
END
GO

-- ============================================
-- ROLLBACK SCRIPT (for reference)
-- ============================================
-- DROP TABLE IF EXISTS TableName;
```

## Dapper Query Patterns

### SELECT with Parameters
```csharp
// Single record
var note = await connection.QuerySingleOrDefaultAsync<Note>(
    "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId",
    new { Id = noteId, UserId = userId }
);

// Multiple records
var notes = await connection.QueryAsync<Note>(
    "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC",
    new { UserId = userId }
);
```

### INSERT with OUTPUT
```csharp
var sql = @"
    INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
    OUTPUT INSERTED.*
    VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt)";

var note = await connection.QuerySingleAsync<Note>(sql, new {
    UserId = userId,
    Title = request.Title,
    Content = request.Content,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
});
```

### UPDATE with OUTPUT
```csharp
var sql = @"
    UPDATE Notes
    SET Title = @Title,
        Content = @Content,
        UpdatedAt = @UpdatedAt
    OUTPUT INSERTED.*
    WHERE Id = @Id AND UserId = @UserId";

var updated = await connection.QuerySingleOrDefaultAsync<Note>(sql, new {
    Id = noteId,
    UserId = userId,
    Title = request.Title,
    Content = request.Content,
    UpdatedAt = DateTime.UtcNow
});
```

### DELETE
```csharp
var rowsAffected = await connection.ExecuteAsync(
    "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId",
    new { Id = noteId, UserId = userId }
);
return rowsAffected > 0;
```

## Search Query Pattern

```csharp
var sql = @"
    SELECT * FROM Notes
    WHERE UserId = @UserId
    AND (@Search IS NULL
         OR Title LIKE '%' + @Search + '%'
         OR Content LIKE '%' + @Search + '%')
    ORDER BY CreatedAt DESC";

var notes = await connection.QueryAsync<Note>(sql, new {
    UserId = userId,
    Search = searchTerm
});
```

## Connection String

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TechbodiaNotesDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

// For Azure SQL
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:server.database.windows.net,1433;Database=TechbodiaNotesDb;User ID=admin;Password=xxx;Encrypt=True;"
  }
}
```

## DbConnectionFactory

```csharp
using System.Data;
using Microsoft.Data.SqlClient;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

// Registration in Program.cs
builder.Services.AddSingleton<DbConnectionFactory>();
```

## Data Constraints

From spec.md:
- Title: Required, 1-200 characters
- Content: Optional, max 50,000 characters
- Timestamps: Auto-generated UTC
- User isolation: All queries filter by UserId

## Performance Considerations

- Index on `UserId` for note lookups
- Composite index on `(UserId, CreatedAt DESC)` for list sorting
- Index on `Email` for user lookup
- Use parameterized queries (prevent SQL injection)
- Connection pooling via SqlConnection

## MCP Integration

- **Context7**: Use for SQL Server, Dapper documentation

## Task Scope

From tasks.md:
- T009: Create Notes table migration
- T010: Create DbConnectionFactory
- T081: Run migrations and seed data

## Validation Checklist

Before completing any task:
- [ ] SQL syntax valid
- [ ] Foreign keys properly defined
- [ ] Indexes created for query patterns
- [ ] Constraints enforce data rules
- [ ] Rollback script documented
- [ ] Parameterized queries (no SQL injection)
- [ ] UTC timestamps used
