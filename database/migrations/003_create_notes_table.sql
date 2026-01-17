-- Migration: 003_create_notes_table
-- Description: Creates the notes table for note management
-- Created: 2026-01-17

-- Create Notes table
CREATE TABLE Notes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Foreign key constraint
    CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE CASCADE,

    -- Ensure title is not empty
    CONSTRAINT CK_Notes_Title_NotEmpty CHECK (LEN(LTRIM(RTRIM(Title))) > 0)
);

-- Create indexes for performance
-- Index for user's notes queries (most common query pattern)
CREATE INDEX IX_Notes_UserId ON Notes(UserId);

-- Index for sorting by dates within a user's notes
CREATE INDEX IX_Notes_UserId_CreatedAt ON Notes(UserId, CreatedAt DESC);
CREATE INDEX IX_Notes_UserId_UpdatedAt ON Notes(UserId, UpdatedAt DESC);

-- Index for title search within user's notes
CREATE INDEX IX_Notes_UserId_Title ON Notes(UserId, Title);

-- Full-text index for content search (optional, requires full-text catalog)
-- CREATE FULLTEXT INDEX ON Notes(Title, Content) KEY INDEX PK_Notes;

-- Add trigger to update UpdatedAt
GO
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
GO
