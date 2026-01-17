# /dotnet-api - ASP.NET Core API Design Skill

Generate ASP.NET Core Web API endpoints with clean architecture for techbodia-note.

## Usage

```
/dotnet-api endpoint CreateNote      # Generate full endpoint stack
/dotnet-api controller Notes         # Generate controller only
/dotnet-api service Notes            # Generate service layer
/dotnet-api repository Notes         # Generate repository layer
/dotnet-api dto CreateNoteRequest    # Generate DTO
/dotnet-api crud Notes               # Generate full CRUD for entity
```

## Arguments

| Arg | Description |
|-----|-------------|
| `endpoint [name]` | Full stack: Controller + Service + Repository |
| `controller [name]` | Controller only |
| `service [name]` | Interface + Implementation |
| `repository [name]` | Interface + Implementation with Dapper |
| `dto [name]` | Request/Response DTO |
| `crud [entity]` | Complete CRUD operations |
| `auth` | Authentication endpoints (register, login, refresh) |

## Output Locations

```
backend/src/TechbodiaNotes.Api/
├── Controllers/
│   ├── AuthController.cs
│   └── NotesController.cs
├── Services/
│   ├── INotesService.cs
│   ├── NotesService.cs
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Repositories/
│   ├── INotesRepository.cs
│   ├── NotesRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Models/
│   ├── Note.cs
│   ├── User.cs
│   └── DTOs/
│       ├── CreateNoteRequest.cs
│       ├── UpdateNoteRequest.cs
│       └── NoteResponse.cs
└── Data/
    └── DbConnectionFactory.cs
```

---

## Templates

### Controller Template

```csharp
// backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechbodiaNotes.Api.Models.DTOs;
using TechbodiaNotes.Api.Services;

namespace TechbodiaNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;
    private readonly ILogger<NotesController> _logger;

    public NotesController(INotesService notesService, ILogger<NotesController> logger)
    {
        _notesService = notesService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notes for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NoteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        var userId = GetCurrentUserId();
        var notes = await _notesService.GetNotesAsync(userId, search, sortBy, sortOrder);
        return Ok(notes);
    }

    /// <summary>
    /// Get a specific note by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NoteResponse>> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var note = await _notesService.GetNoteByIdAsync(userId, id);

        if (note == null)
            return NotFound(new { message = "Note not found" });

        return Ok(note);
    }

    /// <summary>
    /// Create a new note
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NoteResponse>> Create([FromBody] CreateNoteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var note = await _notesService.CreateNoteAsync(userId, request);

        _logger.LogInformation("User {UserId} created note {NoteId}", userId, note.Id);

        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    /// <summary>
    /// Update an existing note
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NoteResponse>> Update(int id, [FromBody] UpdateNoteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var note = await _notesService.UpdateNoteAsync(userId, id, request);

        if (note == null)
            return NotFound(new { message = "Note not found" });

        _logger.LogInformation("User {UserId} updated note {NoteId}", userId, id);

        return Ok(note);
    }

    /// <summary>
    /// Delete a note
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var deleted = await _notesService.DeleteNoteAsync(userId, id);

        if (!deleted)
            return NotFound(new { message = "Note not found" });

        _logger.LogInformation("User {UserId} deleted note {NoteId}", userId, id);

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
```

### Service Interface Template

```csharp
// backend/src/TechbodiaNotes.Api/Services/INotesService.cs
using TechbodiaNotes.Api.Models.DTOs;

namespace TechbodiaNotes.Api.Services;

public interface INotesService
{
    Task<IEnumerable<NoteResponse>> GetNotesAsync(
        int userId,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null);

    Task<NoteResponse?> GetNoteByIdAsync(int userId, int noteId);

    Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request);

    Task<NoteResponse?> UpdateNoteAsync(int userId, int noteId, UpdateNoteRequest request);

    Task<bool> DeleteNoteAsync(int userId, int noteId);
}
```

### Service Implementation Template

```csharp
// backend/src/TechbodiaNotes.Api/Services/NotesService.cs
using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.Models.DTOs;
using TechbodiaNotes.Api.Repositories;

namespace TechbodiaNotes.Api.Services;

public class NotesService : INotesService
{
    private readonly INotesRepository _repository;
    private readonly ILogger<NotesService> _logger;

    public NotesService(INotesRepository repository, ILogger<NotesService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<NoteResponse>> GetNotesAsync(
        int userId,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        var notes = await _repository.GetAllByUserIdAsync(userId, search, sortBy, sortOrder);
        return notes.Select(MapToResponse);
    }

    public async Task<NoteResponse?> GetNoteByIdAsync(int userId, int noteId)
    {
        var note = await _repository.GetByIdAsync(noteId);

        if (note == null || note.UserId != userId)
            return null;

        return MapToResponse(note);
    }

    public async Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request)
    {
        var note = new Note
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Content = request.Content?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(note);
        return MapToResponse(created);
    }

    public async Task<NoteResponse?> UpdateNoteAsync(int userId, int noteId, UpdateNoteRequest request)
    {
        var existingNote = await _repository.GetByIdAsync(noteId);

        if (existingNote == null || existingNote.UserId != userId)
            return null;

        existingNote.Title = request.Title.Trim();
        existingNote.Content = request.Content?.Trim();
        existingNote.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existingNote);
        return updated != null ? MapToResponse(updated) : null;
    }

    public async Task<bool> DeleteNoteAsync(int userId, int noteId)
    {
        var note = await _repository.GetByIdAsync(noteId);

        if (note == null || note.UserId != userId)
            return false;

        return await _repository.DeleteAsync(noteId);
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

### Repository Interface Template

```csharp
// backend/src/TechbodiaNotes.Api/Repositories/INotesRepository.cs
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public interface INotesRepository
{
    Task<IEnumerable<Note>> GetAllByUserIdAsync(
        int userId,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null);

    Task<Note?> GetByIdAsync(int id);

    Task<Note> CreateAsync(Note note);

    Task<Note?> UpdateAsync(Note note);

    Task<bool> DeleteAsync(int id);
}
```

### Repository Implementation Template (Dapper)

```csharp
// backend/src/TechbodiaNotes.Api/Repositories/NotesRepository.cs
using Dapper;
using TechbodiaNotes.Api.Data;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class NotesRepository : INotesRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public NotesRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Note>> GetAllByUserIdAsync(
        int userId,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Validate sort column to prevent SQL injection
        var validSortColumns = new[] { "createdAt", "updatedAt", "title" };
        var sortColumn = validSortColumns.Contains(sortBy?.ToLower())
            ? sortBy
            : "CreatedAt";

        var order = sortOrder?.ToUpper() == "ASC" ? "ASC" : "DESC";

        var sql = $@"
            SELECT * FROM Notes
            WHERE UserId = @UserId
            AND (@Search IS NULL
                 OR Title LIKE '%' + @Search + '%'
                 OR Content LIKE '%' + @Search + '%')
            ORDER BY {sortColumn} {order}";

        return await connection.QueryAsync<Note>(sql, new { UserId = userId, Search = search });
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE Id = @Id",
            new { Id = id }
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

    public async Task<Note?> UpdateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Notes
            SET Title = @Title,
                Content = @Content,
                UpdatedAt = @UpdatedAt
            OUTPUT INSERTED.*
            WHERE Id = @Id";

        return await connection.QuerySingleOrDefaultAsync<Note>(sql, note);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            "DELETE FROM Notes WHERE Id = @Id",
            new { Id = id }
        );

        return rowsAffected > 0;
    }
}
```

### DTO Templates

```csharp
// backend/src/TechbodiaNotes.Api/Models/DTOs/CreateNoteRequest.cs
using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.Models.DTOs;

public class CreateNoteRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(50000, ErrorMessage = "Content must not exceed 50,000 characters")]
    public string? Content { get; set; }
}

// backend/src/TechbodiaNotes.Api/Models/DTOs/UpdateNoteRequest.cs
using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.Models.DTOs;

public class UpdateNoteRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(50000, ErrorMessage = "Content must not exceed 50,000 characters")]
    public string? Content { get; set; }
}

// backend/src/TechbodiaNotes.Api/Models/DTOs/NoteResponse.cs
namespace TechbodiaNotes.Api.Models.DTOs;

public class NoteResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Domain Model Template

```csharp
// backend/src/TechbodiaNotes.Api/Models/Note.cs
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

### Program.cs Configuration

```csharp
// backend/src/TechbodiaNotes.Api/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TechbodiaNotes.Api.Data;
using TechbodiaNotes.Api.Repositories;
using TechbodiaNotes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechbodiaNotes API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddSingleton<DbConnectionFactory>();

// Repositories
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Workflow

1. Parse endpoint/entity name from arguments
2. Generate appropriate layer(s) based on command
3. Follow Controller → Service → Repository pattern
4. Include proper validation attributes on DTOs
5. Add XML documentation for Swagger
6. Register services in Program.cs
7. Include user data isolation (filter by UserId)
