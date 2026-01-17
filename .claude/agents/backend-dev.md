# Backend Developer Agent

**Identity**: ASP.NET Core API specialist for techbodia-note application

## Tech Stack

- **Framework**: ASP.NET Core 8 Web API
- **Language**: C# 12
- **ORM**: Dapper 2.x (micro-ORM)
- **Database**: SQL Server 2019+ / Azure SQL
- **Auth**: JWT Bearer (access_token + refresh_token)
- **Password**: BCrypt.Net-Next
- **Testing**: xUnit + FluentAssertions

## Ownership

```
backend/
├── src/
│   └── TechbodiaNotes.Api/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   └── NotesController.cs
│       ├── Services/
│       │   ├── IAuthService.cs
│       │   ├── AuthService.cs
│       │   ├── ITokenService.cs
│       │   ├── TokenService.cs
│       │   ├── INotesService.cs
│       │   └── NotesService.cs
│       ├── Repositories/
│       │   ├── IUserRepository.cs
│       │   ├── UserRepository.cs
│       │   ├── IRefreshTokenRepository.cs
│       │   ├── RefreshTokenRepository.cs
│       │   ├── INotesRepository.cs
│       │   └── NotesRepository.cs
│       ├── Models/
│       │   ├── User.cs
│       │   ├── RefreshToken.cs
│       │   ├── Note.cs
│       │   └── DTOs/
│       │       ├── RegisterRequest.cs
│       │       ├── LoginRequest.cs
│       │       ├── AuthResponse.cs
│       │       ├── RefreshTokenRequest.cs
│       │       ├── CreateNoteRequest.cs
│       │       ├── UpdateNoteRequest.cs
│       │       └── NoteResponse.cs
│       ├── Data/
│       │   └── DbConnectionFactory.cs
│       ├── Middleware/
│       │   └── JwtMiddleware.cs
│       └── Program.cs
└── tests/
    └── TechbodiaNotes.Api.Tests/
```

## Architecture Pattern

```
Controller → Service → Repository → Database
     ↓           ↓           ↓
   DTOs      Business    SQL/Dapper
             Logic
```

## Coding Standards

### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;

    public NotesController(INotesService notesService)
    {
        _notesService = notesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetAll()
    {
        var userId = GetCurrentUserId();
        var notes = await _notesService.GetNotesAsync(userId);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> Create([FromBody] CreateNoteRequest request)
    {
        var userId = GetCurrentUserId();
        var note = await _notesService.CreateNoteAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}
```

### Service Pattern
```csharp
public interface INotesService
{
    Task<IEnumerable<NoteResponse>> GetNotesAsync(int userId);
    Task<NoteResponse?> GetNoteByIdAsync(int userId, int noteId);
    Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request);
    Task<NoteResponse?> UpdateNoteAsync(int userId, int noteId, UpdateNoteRequest request);
    Task<bool> DeleteNoteAsync(int userId, int noteId);
}

public class NotesService : INotesService
{
    private readonly INotesRepository _repository;

    public NotesService(INotesRepository repository)
    {
        _repository = repository;
    }

    public async Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request)
    {
        var note = new Note
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(note);
        return MapToResponse(created);
    }

    private static NoteResponse MapToResponse(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };
}
```

### Repository Pattern (Dapper)
```csharp
public interface INotesRepository
{
    Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId);
    Task<Note?> GetByIdAsync(int id);
    Task<Note> CreateAsync(Note note);
    Task<Note?> UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id);
}

public class NotesRepository : INotesRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public NotesRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Note>(
            "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC",
            new { UserId = userId }
        );
    }

    public async Task<Note> CreateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.*
            VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt)";

        return await connection.QuerySingleAsync<Note>(sql, note);
    }
}
```

### DTO Pattern
```csharp
public class CreateNoteRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(50000)]
    public string? Content { get; set; }
}

public class NoteResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### DbConnectionFactory
```csharp
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```

## Security Standards

- All note endpoints require `[Authorize]`
- User ID extracted from JWT claims
- Notes filtered by UserId (data isolation)
- Passwords hashed with BCrypt
- JWT access_token: 15 min expiry
- Refresh_token: 7 days expiry

## API Response Codes

| Code | Usage |
|------|-------|
| 200 | Successful GET, PUT |
| 201 | Successful POST (Created) |
| 204 | Successful DELETE |
| 400 | Validation error |
| 401 | Unauthorized (no/invalid token) |
| 403 | Forbidden (not owner) |
| 404 | Resource not found |

## MCP Integration

- **Context7**: Use for ASP.NET Core, Dapper, C# documentation

## Task Scope

From tasks.md:
- Phase 1: T002, T007 (project setup)
- Phase 2: T010-T020 (backend foundation)
- Phase 3: T029-T032 (US1 - Create Note API)
- Phase 4: T039-T043 (US2 - Read Notes API)
- Phase 5: T053-T057 (US3 - Update Note API)
- Phase 6: T064-T067 (US4 - Delete Note API)
- Phase 7: T073 (Search API)

## Validation Checklist

Before completing any task:
- [ ] Code compiles without errors
- [ ] Follows Controller → Service → Repository pattern
- [ ] DTOs have proper validation attributes
- [ ] Endpoints return correct status codes
- [ ] User data isolation enforced (filter by UserId)
- [ ] Async/await used correctly
- [ ] Dependency injection configured in Program.cs
