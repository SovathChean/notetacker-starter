# Data Model: Notes CRUD Operations

**Feature**: 001-notes-crud
**Date**: 2026-01-17
**Status**: Complete

## Entity Relationship Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                          User                                │
├─────────────────────────────────────────────────────────────┤
│ Id           : int (PK, Identity)                           │
│ Email        : nvarchar(255) (Unique, Not Null)             │
│ PasswordHash : nvarchar(500) (Not Null)                     │
│ CreatedAt    : datetime2 (Not Null, Default: GETUTCDATE())  │
└─────────────────────────────────────────────────────────────┘
                              │
            ┌─────────────────┼─────────────────┐
            │ 1:N             │ 1:N             │
            ▼                 ▼                 │
┌───────────────────────────────────┐          │
│          RefreshToken             │          │
├───────────────────────────────────┤          │
│ Id        : int (PK, Identity)    │          │
│ UserId    : int (FK → User.Id)    │          │
│ Token     : nvarchar(500) (Unique)│          │
│ ExpiresAt : datetime2 (Not Null)  │          │
│ IsRevoked : bit (Default: 0)      │          │
│ CreatedAt : datetime2 (Not Null)  │          │
└───────────────────────────────────┘          │
                                               │
┌─────────────────────────────────────────────────────────────┐
│                          Note                                │
├─────────────────────────────────────────────────────────────┤
│ Id          : int (PK, Identity)                            │
│ UserId      : int (FK → User.Id, Not Null)                  │
│ Title       : nvarchar(200) (Not Null)                      │
│ Content     : nvarchar(max) (Null, max 50000 chars)         │
│ CreatedAt   : datetime2 (Not Null, Default: GETUTCDATE())   │
│ UpdatedAt   : datetime2 (Not Null, Default: GETUTCDATE())   │
└─────────────────────────────────────────────────────────────┘
```

## Entities

### Note

The primary entity representing a user's note.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | PK, Identity | Unique identifier |
| UserId | int | FK, Not Null | Owner reference (User.Id) |
| Title | nvarchar(200) | Not Null | Note title (required, max 200 chars) |
| Content | nvarchar(max) | Null | Note content (optional, max 50,000 chars) |
| CreatedAt | datetime2 | Not Null, Default | Auto-generated on creation (UTC) |
| UpdatedAt | datetime2 | Not Null, Default | Auto-updated on modification (UTC) |

**Indexes**:
- `PK_Notes` on Id (clustered)
- `IX_Notes_UserId` on UserId (for filtering by user)
- `IX_Notes_UserId_CreatedAt` on (UserId, CreatedAt DESC) (for sorted listing)

**Constraints**:
- `FK_Notes_Users` foreign key to Users.Id
- `CHK_Notes_Title_Length` check constraint: LEN(Title) > 0 AND LEN(Title) <= 200
- `CHK_Notes_Content_Length` check constraint: Content IS NULL OR LEN(Content) <= 50000

### User

The user entity for authentication and note ownership.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | PK, Identity | Unique identifier |
| Email | nvarchar(255) | Unique, Not Null | User email address |
| PasswordHash | nvarchar(500) | Not Null | BCrypt hashed password |
| CreatedAt | datetime2 | Not Null, Default | Account creation timestamp (UTC) |

**Indexes**:
- `PK_Users` on Id (clustered)
- `IX_Users_Email` on Email (unique, for login lookup)

**Constraints**:
- `CHK_Users_Email_Format` check constraint: Email contains '@'

### RefreshToken

Stores refresh tokens for JWT token renewal.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | PK, Identity | Unique identifier |
| UserId | int | FK, Not Null | Owner reference (User.Id) |
| Token | nvarchar(500) | Unique, Not Null | Hashed refresh token value |
| ExpiresAt | datetime2 | Not Null | Token expiration timestamp (UTC) |
| IsRevoked | bit | Not Null, Default: 0 | Whether token has been revoked |
| CreatedAt | datetime2 | Not Null, Default | Token creation timestamp (UTC) |

**Indexes**:
- `PK_RefreshTokens` on Id (clustered)
- `IX_RefreshTokens_Token` on Token (unique, for token lookup)
- `IX_RefreshTokens_UserId` on UserId (for user's tokens lookup)

**Constraints**:
- `FK_RefreshTokens_Users` foreign key to Users.Id

**Note**: One user can have multiple refresh tokens (supporting multiple devices/sessions).

## SQL Server Schema

### Migration 001: Create Users Table

```sql
-- 001_create_users_table.sql
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CHK_Users_Email_Format CHECK (Email LIKE '%@%.%')
);

-- Index for email lookup (login)
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
```

### Migration 002: Create RefreshTokens Table

```sql
-- 002_create_refresh_tokens_table.sql
CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_RefreshTokens_Token UNIQUE (Token)
);

-- Indexes for performance
CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_UserId_ExpiresAt ON RefreshTokens(UserId, ExpiresAt DESC);
```

### Migration 003: Create Notes Table

```sql
-- 003_create_notes_table.sql
CREATE TABLE Notes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_Notes_Title_Length CHECK (LEN(Title) > 0 AND LEN(Title) <= 200),
    CONSTRAINT CHK_Notes_Content_Length CHECK (Content IS NULL OR LEN(Content) <= 50000)
);

-- Indexes for performance
CREATE INDEX IX_Notes_UserId ON Notes(UserId);
CREATE INDEX IX_Notes_UserId_CreatedAt ON Notes(UserId, CreatedAt DESC);

-- Full-text index for search (optional, improves search performance)
-- CREATE FULLTEXT CATALOG NotesCatalog AS DEFAULT;
-- CREATE FULLTEXT INDEX ON Notes(Title, Content) KEY INDEX PK_Notes;
```

### Seed Data (for development only)

```sql
-- Note: In production, users register through the API
-- This seed data is for development/testing purposes only

-- Insert test user (password: "Password123")
-- BCrypt hash for "Password123" with cost factor 12
INSERT INTO Users (Email, PasswordHash) VALUES
('demo@techbodia.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.AWUl3KGVGDyW3C');

-- Insert sample notes for test user
INSERT INTO Notes (UserId, Title, Content) VALUES
(1, 'Welcome to Techbodia Notes', 'This is your first note. Feel free to edit or delete it.'),
(1, 'Meeting Notes', 'Discuss project timeline and deliverables.'),
(1, 'Shopping List', 'Milk, Bread, Eggs, Coffee');
```

## C# Domain Models

### User.cs

```csharp
namespace TechbodiaNotes.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### RefreshToken.cs

```csharp
namespace TechbodiaNotes.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Note.cs

```csharp
namespace TechbodiaNotes.Api.Models;

public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Auth DTOs

```csharp
// RegisterRequest.cs
namespace TechbodiaNotes.Api.Models.DTOs;

public record RegisterRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    string Password
);

// LoginRequest.cs
public record LoginRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    string Password
);

// AuthResponse.cs
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);

// RefreshTokenRequest.cs
public record RefreshTokenRequest(
    [Required(ErrorMessage = "Refresh token is required")]
    string RefreshToken
);
```

### Notes DTOs

```csharp
// CreateNoteRequest.cs
namespace TechbodiaNotes.Api.Models.DTOs;

public record CreateNoteRequest(
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    string Title,

    [StringLength(50000, ErrorMessage = "Content must be 50,000 characters or less")]
    string? Content
);

// UpdateNoteRequest.cs
public record UpdateNoteRequest(
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    string Title,

    [StringLength(50000, ErrorMessage = "Content must be 50,000 characters or less")]
    string? Content
);

// NoteResponse.cs
public record NoteResponse(
    int Id,
    string Title,
    string? Content,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// NotesListResponse.cs (for paginated results)
public record NotesListResponse(
    IEnumerable<NoteResponse> Notes,
    int TotalCount
);
```

## TypeScript Types

### auth.ts

```typescript
// Request types
export interface RegisterRequest {
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

// Response types
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string; // ISO 8601 date string
  refreshTokenExpiresAt: string; // ISO 8601 date string
}

// User state (for frontend store)
export interface AuthState {
  isAuthenticated: boolean;
  accessToken: string | null;
  refreshToken: string | null;
  accessTokenExpiresAt: string | null;
  refreshTokenExpiresAt: string | null;
}
```

### note.ts

```typescript
// Domain types
export interface Note {
  id: number;
  title: string;
  content: string | null;
  createdAt: string; // ISO 8601 date string
  updatedAt: string; // ISO 8601 date string
}

// Request types
export interface CreateNoteRequest {
  title: string;
  content?: string;
}

export interface UpdateNoteRequest {
  title: string;
  content?: string;
}

// Query parameters for list endpoint
export interface NotesQueryParams {
  search?: string;
  sortBy?: 'createdAt' | 'updatedAt' | 'title';
  sortOrder?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

// Response types
export interface NotesListResponse {
  notes: Note[];
  totalCount: number;
}

// API error response
export interface ApiError {
  message: string;
  code?: string;
  errors?: Record<string, string[]>;
}
```

## Validation Rules

### Authentication Validation (Frontend)

| Field | Rule | Error Message |
|-------|------|---------------|
| Email | Required | "Email is required" |
| Email | Valid format | "Invalid email format" |
| Password | Required | "Password is required" |
| Password | Min 8 chars | "Password must be at least 8 characters" |

### Notes Validation (Frontend)

| Field | Rule | Error Message |
|-------|------|---------------|
| Title | Required | "Title is required" |
| Title | Max 200 chars | "Title must be 200 characters or less" |
| Content | Max 50,000 chars | "Content must be 50,000 characters or less" |

### Backend (C# Data Annotations)

See DTOs section above for complete validation annotations.

## State Transitions

### Authentication Lifecycle

```
[Guest] ──Register──▶ [Authenticated]
    │                       │
    │                       ├──Logout──▶ [Guest]
    │                       │
    │                       ├──TokenExpired──▶ [Token Refresh] ──Success──▶ [Authenticated]
    │                       │                       │
    │                       │                       └──Failed──▶ [Guest]
    │                       │
    └───────Login───────────┘
```

**Authentication State Invariants**:
- `access_token` expires after 15 minutes
- `refresh_token` expires after 7 days
- Logout revokes all refresh tokens for the user
- Invalid refresh token redirects to login

### Note Lifecycle

```
[Not Exists] ──Create──▶ [Created]
                             │
                             ├──Update──▶ [Updated] ──┐
                             │                        │
                             │◀───────────────────────┘
                             │
                             └──Delete──▶ [Deleted/Not Exists]
```

**Note State Invariants**:
- `CreatedAt` is set once on creation and never changes
- `UpdatedAt` is set on creation and updated on every modification
- `UpdatedAt >= CreatedAt` always holds true
- Notes are only accessible by their owner (UserId must match authenticated user)
