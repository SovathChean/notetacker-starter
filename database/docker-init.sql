-- Docker Database Initialization Script
-- Creates database and runs all migrations

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TechbodiaNotesDb')
BEGIN
    CREATE DATABASE TechbodiaNotesDb;
    PRINT 'Database TechbodiaNotesDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database TechbodiaNotesDb already exists.';
END
GO

USE TechbodiaNotesDb;
GO

-- Migration: 001_create_users_table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Email NVARCHAR(255) NOT NULL,
        Username NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );

    CREATE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_Username ON Users(Username);
    CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt DESC);

    PRINT 'Table Users created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Users already exists.';
END
GO

-- Create trigger for Users UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Users_UpdatedAt')
BEGIN
    EXEC('
    CREATE TRIGGER TR_Users_UpdatedAt
    ON Users
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Users
        SET UpdatedAt = GETUTCDATE()
        FROM Users u
        INNER JOIN inserted i ON u.Id = i.Id;
    END;
    ');
    PRINT 'Trigger TR_Users_UpdatedAt created successfully.';
END
GO

-- Migration: 002_create_refresh_tokens_table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Token NVARCHAR(500) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        RevokedAt DATETIME2 NULL,
        ReplacedByToken NVARCHAR(500) NULL,
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
            REFERENCES Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
    CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
    CREATE INDEX IX_RefreshTokens_Cleanup ON RefreshTokens(ExpiresAt, RevokedAt);

    PRINT 'Table RefreshTokens created successfully.';
END
ELSE
BEGIN
    PRINT 'Table RefreshTokens already exists.';
END
GO

-- Migration: 003_create_notes_table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
BEGIN
    CREATE TABLE Notes (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId)
            REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT CK_Notes_Title_NotEmpty CHECK (LEN(LTRIM(RTRIM(Title))) > 0)
    );

    CREATE INDEX IX_Notes_UserId ON Notes(UserId);
    CREATE INDEX IX_Notes_UserId_CreatedAt ON Notes(UserId, CreatedAt DESC);
    CREATE INDEX IX_Notes_UserId_UpdatedAt ON Notes(UserId, UpdatedAt DESC);
    CREATE INDEX IX_Notes_UserId_Title ON Notes(UserId, Title);

    PRINT 'Table Notes created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Notes already exists.';
END
GO

-- Create trigger for Notes UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Notes_UpdatedAt')
BEGIN
    EXEC('
    CREATE TRIGGER TR_Notes_UpdatedAt
    ON Notes
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Notes
        SET UpdatedAt = GETUTCDATE()
        FROM Notes n
        INNER JOIN inserted i ON n.Id = i.Id;
    END;
    ');
    PRINT 'Trigger TR_Notes_UpdatedAt created successfully.';
END
GO

PRINT 'Database initialization completed successfully!';
GO
