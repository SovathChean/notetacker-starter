# Research: Notes CRUD Operations

**Feature**: 001-notes-crud
**Date**: 2026-01-17
**Status**: Complete

## Technology Decisions

### 1. Frontend Framework: Vue 3 + Composition API

**Decision**: Use Vue 3 with Composition API and `<script setup>` syntax

**Rationale**:
- Constitution mandates Vue 3 with Composition API
- `<script setup>` provides cleaner, more concise component code
- Better TypeScript integration than Options API
- Enables composables for reusable logic (useNotes, useSearch)

**Alternatives Considered**:
- Options API: Rejected due to verbose syntax and weaker TypeScript support
- React: Not considered (project constitution specifies Vue)

### 2. State Management: Pinia

**Decision**: Use Pinia for global state management

**Rationale**:
- Official Vue 3 state management solution
- First-class TypeScript support
- Simpler API than Vuex
- Devtools integration for debugging
- Modular store design fits notes feature

**Alternatives Considered**:
- Vuex: More boilerplate, legacy solution
- Vue's reactive(): Sufficient for simple cases but lacks devtools and persistence helpers

### 3. API Communication: Axios

**Decision**: Use Axios for HTTP requests

**Rationale**:
- Automatic JSON transformation
- Request/response interceptors for error handling
- Cancellation support for search debouncing
- Wide browser support
- Easy to configure base URL and headers

**Alternatives Considered**:
- Fetch API: Native but requires more boilerplate for error handling and interceptors
- ky: Lighter but less ecosystem support

### 4. Backend Architecture: Clean Architecture with Dapper

**Decision**: Controllers → Services → Repositories pattern with Dapper

**Rationale**:
- Constitution Principle III mandates layered architecture
- Dapper provides lightweight ORM with full SQL control
- Repository pattern centralizes data access logic
- Service layer contains business logic
- Easy to test each layer independently

**Alternatives Considered**:
- Entity Framework Core: More abstraction but heavier; Dapper preferred per requirements
- Direct SQL in services: Violates separation of concerns

### 5. User Data Isolation Strategy

**Decision**: Filter by UserId at repository level for all queries

**Rationale**:
- Constitution Principle I mandates user-scoped data access
- Repository methods accept userId parameter
- All SELECT/UPDATE/DELETE queries include WHERE UserId = @UserId
- API returns 403 Forbidden for unauthorized access attempts

**Implementation**:
```csharp
// Repository pattern ensures all queries are user-scoped
public async Task<IEnumerable<Note>> GetAllAsync(int userId)
{
    const string sql = "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
    return await _connection.QueryAsync<Note>(sql, new { UserId = userId });
}
```

### 6. Search, Filter, and Sort Implementation

**Decision**: Server-side filtering and sorting with query parameters

**Rationale**:
- Handles large datasets efficiently
- Reduces data transfer
- Consistent behavior across devices
- Enables future pagination

**Query Parameters**:
- `search`: Text search in title and content
- `sortBy`: Field name (createdAt, updatedAt, title)
- `sortOrder`: asc or desc
- `page` and `pageSize`: For pagination (future)

### 7. Responsive Design Approach

**Decision**: Mobile-first with TailwindCSS responsive utilities

**Rationale**:
- Constitution Principle V mandates responsive design
- TailwindCSS utilities (sm:, md:, lg:) for breakpoints
- Mobile-first ensures good experience on all devices

**Breakpoint Strategy**:
- Mobile: < 640px (default)
- Tablet: 640px - 1024px (sm:, md:)
- Desktop: > 1024px (lg:, xl:)

### 8. Form Validation Strategy

**Decision**: Dual validation (frontend + backend)

**Rationale**:
- Frontend: Immediate user feedback
- Backend: Security and data integrity
- Both enforce same rules (title required, length limits)

**Frontend**: Vue form validation with reactive rules
**Backend**: Data annotations on DTOs + FluentValidation

## Best Practices Applied

### Vue 3 / TypeScript

1. **Strict TypeScript**: Enable `strict: true` in tsconfig.json
2. **Type Exports**: Define interfaces in `types/note.ts` for reuse
3. **Composables**: Extract reusable logic (useNotes, useSearch)
4. **Props Validation**: Use `defineProps<T>()` with TypeScript generics

### ASP.NET Core / Dapper

1. **Async/Await**: All database operations are async
2. **Parameterized Queries**: Prevent SQL injection
3. **DTOs**: Separate request/response models from domain entities
4. **Dependency Injection**: All services registered in DI container
5. **Nullable Reference Types**: Enable in project file

### API Design

1. **RESTful Routes**: Standard CRUD endpoints
2. **HTTP Status Codes**: 200, 201, 204, 400, 403, 404, 500
3. **Error Responses**: Consistent error format with message and details
4. **Validation Errors**: Return 400 with field-level error messages

## Resolved Clarifications

All technical decisions have been made. No outstanding clarifications needed.

| Topic | Resolution |
|-------|------------|
| State management library | Pinia (Vue 3 official solution) |
| HTTP client | Axios (interceptors, TypeScript support) |
| Search implementation | Server-side with query parameters |
| Pagination | Deferred to future enhancement (spec assumes < 500 notes) |
| Authentication | Optional for junior scope; UserId will be hardcoded or from simple auth |
