# /database - SQL Server Database Design Skill

Generate SQL Server database schemas and migrations for techbodia-note.

## Usage

```
/database migration create_notes     # Create migration script
/database schema                     # Generate full schema
/database table Notes                # Create single table
/database index Notes UserId         # Create index
/database seed                       # Generate seed data
/database query GetNotesByUser       # Generate Dapper query
```

## Arguments

| Arg | Description |
|-----|-------------|
| `migration [name]` | Create numbered migration script |
| `schema` | Generate complete database schema |
| `table [name]` | Create single table DDL |
| `index [table] [column]` | Create index |
| `seed` | Generate sample data inserts |
| `query [name]` | Generate Dapper query template |
| `backup` | Generate backup script |
| `rollback [migration]` | Generate rollback script |

## Output Locations

```
database/
└── migrations/
    ├── 001_create_users_table.sql
    ├── 002_create_refresh_tokens_table.sql
    ├── 003_create_notes_table.sql
    ├── 004_add_indexes.sql
    └── seed_data.sql

backend/src/TechbodiaNotes.Api/
└── Data/
    └── DbConnectionFactory.cs
```

---

## Schema Overview

Based on data-model.md:

```
┌─────────────┐       ┌─────────────────┐       ┌─────────────┐
│   Users     │───1:N─│ RefreshTokens   │       │    Notes    │
│─────────────│       │─────────────────│       │─────────────│
│ Id (PK)     │       │ Id (PK)         │       │ Id (PK)     │
│ Email       │───────│ UserId (FK)     │       │ UserId (FK) │──┐
│ PasswordHash│       │ TokenHash       │       │ Title       │  │
│ CreatedAt   │       │ ExpiresAt       │       │ Content     │  │
└─────────────┘       │ IsRevoked       │       │ CreatedAt   │  │
                      │ CreatedAt       │       │ UpdatedAt   │  │
                      └─────────────────┘       └─────────────┘  │
                                                       │         │
                                                       └─────────┘
                                                         1:N
```

---

## Migration Templates

### 001_create_users_table.sql

```sql
-- Migration: 001_create_users_table.sql
-- Date: 2026-01-17
-- Description: Create Users table for authentication

-- ============================================
-- UP MIGRATION
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        -- Constraints
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_Email_Format CHECK (Email LIKE '%_@_%._%')
    );

    -- Indexes
    CREATE INDEX IX_Users_Email ON Users(Email);

    PRINT 'Table Users created successfully';
END
ELSE
BEGIN
    PRINT 'Table Users already exists';
END
GO

-- ============================================
-- ROLLBACK (for reference)
-- ============================================
-- DROP TABLE IF EXISTS Users;
```

### 002_create_refresh_tokens_table.sql

```sql
-- Migration: 002_create_refresh_tokens_table.sql
-- Date: 2026-01-17
-- Description: Create RefreshTokens table for JWT token management

-- ============================================
-- UP MIGRATION
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        TokenHash NVARCHAR(255) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        IsRevoked BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        RevokedAt DATETIME2 NULL,

        -- Foreign Key
        CONSTRAINT FK_RefreshTokens_Users
            FOREIGN KEY (UserId)
            REFERENCES Users(Id)
            ON DELETE CASCADE
    );

    -- Indexes
    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
    CREATE INDEX IX_RefreshTokens_TokenHash ON RefreshTokens(TokenHash);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt)
        WHERE IsRevoked = 0;

    PRINT 'Table RefreshTokens created successfully';
END
GO

-- ============================================
-- ROLLBACK
-- ============================================
-- DROP TABLE IF EXISTS RefreshTokens;
```

### 003_create_notes_table.sql

```sql
-- Migration: 003_create_notes_table.sql
-- Date: 2026-01-17
-- Description: Create Notes table for storing user notes

-- ============================================
-- UP MIGRATION
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
BEGIN
    CREATE TABLE Notes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        -- Foreign Key
        CONSTRAINT FK_Notes_Users
            FOREIGN KEY (UserId)
            REFERENCES Users(Id)
            ON DELETE CASCADE,

        -- Check Constraints (per spec.md)
        CONSTRAINT CK_Notes_Title_NotEmpty
            CHECK (LEN(LTRIM(RTRIM(Title))) >= 1),
        CONSTRAINT CK_Notes_Title_MaxLength
            CHECK (LEN(Title) <= 200),
        CONSTRAINT CK_Notes_Content_MaxLength
            CHECK (Content IS NULL OR LEN(Content) <= 50000)
    );

    -- Indexes for performance
    CREATE INDEX IX_Notes_UserId ON Notes(UserId);

    -- Composite index for list queries (newest first)
    CREATE INDEX IX_Notes_UserId_CreatedAt
        ON Notes(UserId, CreatedAt DESC);

    -- Index for search queries
    CREATE INDEX IX_Notes_UserId_Title
        ON Notes(UserId, Title);

    PRINT 'Table Notes created successfully';
END
GO

-- ============================================
-- ROLLBACK
-- ============================================
-- DROP TABLE IF EXISTS Notes;
```

### 004_add_full_text_search.sql (Optional)

```sql
-- Migration: 004_add_full_text_search.sql
-- Date: 2026-01-17
-- Description: Add full-text search capability for notes

-- ============================================
-- UP MIGRATION
-- ============================================

-- Create full-text catalog
IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'NotesCatalog')
BEGIN
    CREATE FULLTEXT CATALOG NotesCatalog AS DEFAULT;
    PRINT 'Full-text catalog created';
END
GO

-- Create full-text index on Notes
IF NOT EXISTS (
    SELECT * FROM sys.fulltext_indexes
    WHERE object_id = OBJECT_ID('Notes')
)
BEGIN
    CREATE FULLTEXT INDEX ON Notes(Title, Content)
    KEY INDEX PK__Notes__Id
    ON NotesCatalog
    WITH CHANGE_TRACKING AUTO;

    PRINT 'Full-text index created on Notes';
END
GO

-- ============================================
-- ROLLBACK
-- ============================================
-- DROP FULLTEXT INDEX ON Notes;
-- DROP FULLTEXT CATALOG NotesCatalog;
```

---

## Seed Data Template

```sql
-- seed_data.sql
-- Description: Insert sample data for development/testing

-- ============================================
-- CLEAR EXISTING DATA (Development Only!)
-- ============================================
-- DELETE FROM Notes;
-- DELETE FROM RefreshTokens;
-- DELETE FROM Users;
-- DBCC CHECKIDENT ('Notes', RESEED, 0);
-- DBCC CHECKIDENT ('Users', RESEED, 0);

-- ============================================
-- INSERT SAMPLE USERS
-- ============================================
-- Password: "Password123" hashed with BCrypt
DECLARE @PasswordHash NVARCHAR(255) = '$2a$11$K.0HwpsoPDGaB/7FHsGb8u5pHCVR.6yZmNb9NZJpFJj4CmPqxL.CG';

INSERT INTO Users (Email, PasswordHash, CreatedAt)
VALUES
    ('demo@example.com', @PasswordHash, GETUTCDATE()),
    ('test@example.com', @PasswordHash, GETUTCDATE());

-- ============================================
-- INSERT SAMPLE NOTES
-- ============================================
DECLARE @DemoUserId INT = (SELECT Id FROM Users WHERE Email = 'demo@example.com');

INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
VALUES
    (@DemoUserId, 'Welcome to TechbodiaNotes',
     'This is your first note. You can edit or delete it anytime.',
     DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, -7, GETUTCDATE())),

    (@DemoUserId, 'Meeting Notes - Project Kickoff',
     'Discussed project timeline and milestones. Next meeting scheduled for Monday.',
     DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, -5, GETUTCDATE())),

    (@DemoUserId, 'Shopping List',
     'Milk, Bread, Eggs, Butter, Coffee, Apples',
     DATEADD(DAY, -3, GETUTCDATE()), DATEADD(DAY, -1, GETUTCDATE())),

    (@DemoUserId, 'Ideas for New Features',
     'Tags for notes, Dark mode, Export to PDF, Markdown support',
     DATEADD(DAY, -1, GETUTCDATE()), GETUTCDATE());

PRINT 'Seed data inserted successfully';
GO
```

---

## Dapper Query Templates

### GetAllByUserId

```csharp
public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId)
{
    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Note>(@"
        SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
        FROM Notes
        WHERE UserId = @UserId
        ORDER BY CreatedAt DESC",
        new { UserId = userId }
    );
}
```

### GetById with Ownership Check

```csharp
public async Task<Note?> GetByIdAsync(int id, int userId)
{
    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<Note>(@"
        SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
        FROM Notes
        WHERE Id = @Id AND UserId = @UserId",
        new { Id = id, UserId = userId }
    );
}
```

### Search with Pagination

```csharp
public async Task<(IEnumerable<Note> Notes, int Total)> SearchAsync(
    int userId,
    string? search,
    int page = 1,
    int pageSize = 20)
{
    using var connection = _connectionFactory.CreateConnection();

    var offset = (page - 1) * pageSize;

    var sql = @"
        SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
        FROM Notes
        WHERE UserId = @UserId
          AND (@Search IS NULL
               OR Title LIKE '%' + @Search + '%'
               OR Content LIKE '%' + @Search + '%')
        ORDER BY CreatedAt DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Notes
        WHERE UserId = @UserId
          AND (@Search IS NULL
               OR Title LIKE '%' + @Search + '%'
               OR Content LIKE '%' + @Search + '%');";

    using var multi = await connection.QueryMultipleAsync(sql, new
    {
        UserId = userId,
        Search = search,
        Offset = offset,
        PageSize = pageSize
    });

    var notes = await multi.ReadAsync<Note>();
    var total = await multi.ReadSingleAsync<int>();

    return (notes, total);
}
```

### Insert with OUTPUT

```csharp
public async Task<Note> CreateAsync(Note note)
{
    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleAsync<Note>(@"
        INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id,
               INSERTED.UserId,
               INSERTED.Title,
               INSERTED.Content,
               INSERTED.CreatedAt,
               INSERTED.UpdatedAt
        VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt)",
        note
    );
}
```

### Update with OUTPUT

```csharp
public async Task<Note?> UpdateAsync(Note note)
{
    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<Note>(@"
        UPDATE Notes
        SET Title = @Title,
            Content = @Content,
            UpdatedAt = @UpdatedAt
        OUTPUT INSERTED.Id,
               INSERTED.UserId,
               INSERTED.Title,
               INSERTED.Content,
               INSERTED.CreatedAt,
               INSERTED.UpdatedAt
        WHERE Id = @Id AND UserId = @UserId",
        note
    );
}
```

### Delete

```csharp
public async Task<bool> DeleteAsync(int id, int userId)
{
    using var connection = _connectionFactory.CreateConnection();

    var rowsAffected = await connection.ExecuteAsync(@"
        DELETE FROM Notes
        WHERE Id = @Id AND UserId = @UserId",
        new { Id = id, UserId = userId }
    );

    return rowsAffected > 0;
}
```

---

## Connection String Templates

### Development (Local SQL Server)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TechbodiaNotesDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Docker (SQL Server Container)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db;Database=TechbodiaNotesDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### Azure SQL

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=TechbodiaNotesDb;User ID=admin;Password=xxx;Encrypt=True;Connection Timeout=30;"
  }
}
```

---

## Workflow

1. Parse command and arguments
2. Generate appropriate SQL/C# code
3. Include proper constraints per spec.md
4. Add indexes for query patterns
5. Include rollback scripts
6. Follow naming conventions
7. Use UTC timestamps (GETUTCDATE())
