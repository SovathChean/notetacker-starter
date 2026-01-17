-- Migration: 002_create_refresh_tokens_table
-- Description: Creates the refresh_tokens table for JWT token management
-- Created: 2026-01-17

-- Create RefreshTokens table
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2 NULL,
    ReplacedByToken NVARCHAR(500) NULL,

    -- Foreign key constraint
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);

-- Index for cleanup queries (find expired or revoked tokens)
CREATE INDEX IX_RefreshTokens_Cleanup ON RefreshTokens(ExpiresAt, RevokedAt);
GO
