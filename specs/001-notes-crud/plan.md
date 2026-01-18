# Implementation Plan: Notes CRUD Operations

**Branch**: `001-notes-crud` | **Date**: 2026-01-17 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-notes-crud/spec.md`

## Summary

Implement a full-stack notes management application with CRUD operations (Create, Read, Update, Delete).
The frontend uses Vue 3 + TypeScript + TailwindCSS with filtering, sorting, and search capabilities.
The backend uses C# ASP.NET Core Web API with Entity Framework Core connecting to SQL Server.
Users can only access their own notes (data isolation enforced at API level).
Authentication is implemented using JWT with OAuth2 pattern (access_token + refresh_token).

## Technical Context

**Language/Version**:
- Frontend: TypeScript 5.x with Vue 3.4+
- Backend: C# 12 with .NET 8

**Primary Dependencies**:
- Frontend: Vue 3, Vue Router, Pinia (state management), Axios, TailwindCSS 3.x
- Backend: ASP.NET Core 8, Entity Framework Core 8.x, Microsoft.Data.SqlClient
- Authentication: Microsoft.AspNetCore.Authentication.JwtBearer, BCrypt.Net-Next

**Storage**: SQL Server (2019+ or Azure SQL)

**Testing**:
- Frontend: Vitest + Vue Test Utils
- Backend: xUnit + FluentAssertions

**Target Platform**: Web (modern browsers: Chrome, Firefox, Safari, Edge)

**Project Type**: Web application (frontend + backend)

**Performance Goals**:
- Notes list loads within 2 seconds for up to 500 notes
- API response time < 200ms for CRUD operations
- Frontend initial load < 3 seconds

**Constraints**:
- Must work on mobile devices (responsive design)
- User data isolation (users see only their own notes)
- Maximum title: 200 characters, content: 50,000 characters

**Scale/Scope**:
- Full user authentication with JWT (access_token + refresh_token)
- Target: Handle up to 500 notes per user
- Pages: Login, Register, Notes list, Note detail/edit, Create note form

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Compliance | Evidence |
|-----------|------------|----------|
| I. Data Ownership & Isolation | ✅ PASS | FR-012 requires user-scoped data; API will filter by UserId |
| II. Type Safety First | ✅ PASS | TypeScript strict mode on frontend; C# DTOs on backend |
| III. Clean Architecture | ✅ PASS | Separate frontend/backend; Controllers → Services → Repositories |
| IV. Reusable & DRY Code | ✅ PASS | Composable Vue components; Injectable services |
| V. Responsive & Accessible UI | ✅ PASS | TailwindCSS responsive utilities; Form labels required |

**Gate Status**: ✅ PASSED - All constitution principles satisfied

## Project Structure

### Documentation (this feature)

```text
specs/001-notes-crud/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (OpenAPI spec)
│   └── api.yaml
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
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
│       ├── Infrastructure/
│       │   └── ApplicationDbContext.cs
│       ├── Middleware/
│       │   └── JwtMiddleware.cs
│       └── Program.cs
└── tests/
    └── TechbodiaNotes.Api.Tests/
        ├── Controllers/
        ├── Services/
        └── Repositories/

frontend/
├── src/
│   ├── components/
│   │   ├── auth/
│   │   │   ├── LoginForm.vue
│   │   │   └── RegisterForm.vue
│   │   ├── notes/
│   │   │   ├── NoteCard.vue
│   │   │   ├── NoteForm.vue
│   │   │   ├── NoteList.vue
│   │   │   └── NoteDetail.vue
│   │   └── common/
│   │       ├── SearchBar.vue
│   │       ├── SortDropdown.vue
│   │       ├── FilterPanel.vue
│   │       ├── ConfirmDialog.vue
│   │       └── LoadingSpinner.vue
│   ├── pages/
│   │   ├── LoginPage.vue
│   │   ├── RegisterPage.vue
│   │   ├── NotesPage.vue
│   │   └── NoteDetailPage.vue
│   ├── services/
│   │   ├── authApi.ts
│   │   └── notesApi.ts
│   ├── stores/
│   │   ├── authStore.ts
│   │   └── notesStore.ts
│   ├── types/
│   │   ├── auth.ts
│   │   └── note.ts
│   ├── composables/
│   │   ├── useAuth.ts
│   │   ├── useNotes.ts
│   │   └── useSearch.ts
│   ├── router/
│   │   └── index.ts
│   ├── App.vue
│   └── main.ts
├── tests/
│   ├── components/
│   └── stores/
├── index.html
├── vite.config.ts
├── tailwind.config.js
├── tsconfig.json
└── package.json

database/
└── migrations/
    ├── 001_create_users_table.sql
    ├── 002_create_refresh_tokens_table.sql
    └── 003_create_notes_table.sql
```

**Structure Decision**: Web application structure with separate `frontend/` and `backend/` directories.
This aligns with Constitution Principle III (Clean Architecture) requiring independently deployable layers.

## Complexity Tracking

> No constitution violations - all principles satisfied with standard patterns.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | N/A | N/A |
