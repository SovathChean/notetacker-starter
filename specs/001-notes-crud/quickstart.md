# Quickstart: Notes CRUD Operations

**Feature**: 001-notes-crud
**Date**: 2026-01-17

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| Node.js | 18.x or 20.x | Frontend runtime |
| npm | 9.x+ | Package manager |
| .NET SDK | 8.0 | Backend runtime |
| SQL Server | 2019+ or Azure SQL | Database |
| VS Code | Latest | Recommended IDE |

### VS Code Extensions (Recommended)

- Vue - Official (Vue.volar)
- TypeScript Vue Plugin (Vue.vscode-typescript-vue-plugin)
- Tailwind CSS IntelliSense
- C# Dev Kit
- REST Client

## Project Setup

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd techbodia-note
git checkout 001-notes-crud
```

### 2. Database Setup

Create the database and tables:

```bash
# Connect to SQL Server and run the migration
sqlcmd -S localhost -d master -Q "CREATE DATABASE TechbodiaNotes"
sqlcmd -S localhost -d TechbodiaNotes -i database/migrations/001_create_notes_table.sql
```

Or using SQL Server Management Studio:
1. Open SSMS and connect to your SQL Server instance
2. Create a new database named `TechbodiaNotes`
3. Run the migration script from `database/migrations/001_create_notes_table.sql`

### 3. Backend Setup

```bash
cd backend/src/TechbodiaNotes.Api

# Restore dependencies
dotnet restore

# Update connection string in appsettings.Development.json
# Default: "Server=localhost;Database=TechbodiaNotes;Trusted_Connection=True;TrustServerCertificate=True"

# Run the API
dotnet run

# API will be available at http://localhost:5000
```

### 4. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev

# Frontend will be available at http://localhost:5173
```

## Verify Installation

### Check Backend Health

```bash
curl http://localhost:5000/api/health
# Expected: {"status":"healthy"}
```

### Check API Endpoints

```bash
# Get all notes (empty list initially)
curl http://localhost:5000/api/notes
# Expected: {"notes":[],"totalCount":0}

# Create a note
curl -X POST http://localhost:5000/api/notes \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Note","content":"Hello World"}'
# Expected: {"id":1,"title":"Test Note","content":"Hello World","createdAt":"...","updatedAt":"..."}
```

### Check Frontend

1. Open http://localhost:5173 in your browser
2. You should see the Notes list page
3. Try creating a new note using the "Create Note" button

## Development Workflow

### Running Tests

```bash
# Backend tests
cd backend/tests/TechbodiaNotes.Api.Tests
dotnet test

# Frontend tests
cd frontend
npm run test
```

### Code Quality

```bash
# Frontend linting
cd frontend
npm run lint

# Frontend type checking
npm run type-check

# Backend build (includes analyzers)
cd backend/src/TechbodiaNotes.Api
dotnet build
```

### API Documentation

The API follows the OpenAPI specification in `specs/001-notes-crud/contracts/api.yaml`.

You can view it using:
- Swagger UI: http://localhost:5000/swagger (when running backend)
- Import into Postman or Insomnia

## Environment Variables

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TechbodiaNotes;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Frontend (.env)

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

## Common Issues

### CORS Errors

If you see CORS errors in the browser console, ensure the backend has CORS configured for the frontend origin:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### SQL Server Connection

If connection fails:
1. Ensure SQL Server is running
2. Check the connection string in appsettings.json
3. For Windows Auth: Ensure your user has database access
4. For SQL Auth: Update connection string with username/password

### Port Conflicts

- Backend default: 5000 (change in `launchSettings.json`)
- Frontend default: 5173 (change in `vite.config.ts`)

## Next Steps

1. Run `/speckit.tasks` to generate the task breakdown
2. Start with Phase 1: Setup tasks
3. Follow the task list in `specs/001-notes-crud/tasks.md`
