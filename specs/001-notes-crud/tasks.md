# Tasks: Notes CRUD Operations

**Input**: Design documents from `/specs/001-notes-crud/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Tests are NOT explicitly requested in the specification. Test tasks are omitted.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4, US5, US6, US7)
- Include exact file paths in descriptions

## Path Conventions

- **Web app structure**: `backend/src/`, `frontend/src/`
- Backend: `backend/src/TechbodiaNotes.Api/`
- Frontend: `frontend/src/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create project directory structure per implementation plan (backend/, frontend/, database/)
- [ ] T002 Initialize .NET 8 Web API project in backend/src/TechbodiaNotes.Api/TechbodiaNotes.Api.csproj
- [ ] T003 [P] Initialize Vue 3 + TypeScript project with Vite in frontend/package.json
- [ ] T004 [P] Configure TailwindCSS in frontend/tailwind.config.js and frontend/src/index.css
- [ ] T005 [P] Configure TypeScript strict mode in frontend/tsconfig.json
- [ ] T006 [P] Configure ESLint and Prettier in frontend/.eslintrc.cjs and frontend/.prettierrc
- [ ] T007 [P] Add .NET dependencies (Entity Framework Core, Microsoft.Data.SqlClient, JwtBearer, BCrypt.Net-Next) in backend/src/TechbodiaNotes.Api/TechbodiaNotes.Api.csproj
- [ ] T008 [P] Add frontend dependencies (Vue Router, Pinia, Axios) in frontend/package.json

**Checkpoint**: Project scaffolding complete - ready for foundational infrastructure

---

## Phase 2: Foundational (Database & Core Infrastructure)

**Purpose**: Database schema and core infrastructure that MUST be complete before ANY user story

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Database Migrations

- [ ] T009 Create SQL migration script in database/migrations/001_create_users_table.sql
- [ ] T010 [P] Create SQL migration script in database/migrations/002_create_refresh_tokens_table.sql
- [ ] T011 [P] Create SQL migration script in database/migrations/003_create_notes_table.sql
- [ ] T012 Create ApplicationDbContext with DbSets in backend/src/TechbodiaNotes.Api/Infrastructure/ApplicationDbContext.cs

### Domain Models

- [ ] T013 [P] Create User domain model in backend/src/TechbodiaNotes.Api/Models/User.cs
- [ ] T014 [P] Create RefreshToken domain model in backend/src/TechbodiaNotes.Api/Models/RefreshToken.cs
- [ ] T015 [P] Create Note domain model in backend/src/TechbodiaNotes.Api/Models/Note.cs

### Auth DTOs

- [ ] T016 [P] Create RegisterRequest DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/RegisterRequest.cs
- [ ] T017 [P] Create LoginRequest DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/LoginRequest.cs
- [ ] T018 [P] Create AuthResponse DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/AuthResponse.cs
- [ ] T019 [P] Create RefreshTokenRequest DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/RefreshTokenRequest.cs

### Notes DTOs

- [ ] T020 [P] Create CreateNoteRequest DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/CreateNoteRequest.cs
- [ ] T021 [P] Create UpdateNoteRequest DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/UpdateNoteRequest.cs
- [ ] T022 [P] Create NoteResponse DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/NoteResponse.cs
- [ ] T023 [P] Create NotesListResponse DTO in backend/src/TechbodiaNotes.Api/Models/DTOs/NotesListResponse.cs

### Auth Repositories

- [ ] T024 Create IUserRepository interface in backend/src/TechbodiaNotes.Api/Repositories/IUserRepository.cs
- [ ] T025 Implement UserRepository with EF Core in backend/src/TechbodiaNotes.Api/Repositories/UserRepository.cs
- [ ] T026 [P] Create IRefreshTokenRepository interface in backend/src/TechbodiaNotes.Api/Repositories/IRefreshTokenRepository.cs
- [ ] T027 [P] Implement RefreshTokenRepository with EF Core in backend/src/TechbodiaNotes.Api/Repositories/RefreshTokenRepository.cs

### Notes Repositories

- [ ] T028 Create INotesRepository interface in backend/src/TechbodiaNotes.Api/Repositories/INotesRepository.cs
- [ ] T029 Implement NotesRepository with EF Core in backend/src/TechbodiaNotes.Api/Repositories/NotesRepository.cs

### Auth Services

- [ ] T030 Create ITokenService interface in backend/src/TechbodiaNotes.Api/Services/ITokenService.cs
- [ ] T031 Implement TokenService (JWT generation, validation) in backend/src/TechbodiaNotes.Api/Services/TokenService.cs
- [ ] T032 Create IAuthService interface in backend/src/TechbodiaNotes.Api/Services/IAuthService.cs
- [ ] T033 Implement AuthService (register, login, refresh, logout) in backend/src/TechbodiaNotes.Api/Services/AuthService.cs

### Notes Services

- [ ] T034 Create INotesService interface in backend/src/TechbodiaNotes.Api/Services/INotesService.cs
- [ ] T035 Implement NotesService in backend/src/TechbodiaNotes.Api/Services/NotesService.cs

### Backend Configuration

- [ ] T036 Create JWT middleware in backend/src/TechbodiaNotes.Api/Middleware/JwtMiddleware.cs
- [ ] T037 Configure DI, CORS, JWT Auth, and Swagger in backend/src/TechbodiaNotes.Api/Program.cs
- [ ] T038 Add JWT settings to appsettings.json (secret, issuer, audience, expiry times)

### Frontend Foundation

- [ ] T039 [P] Create TypeScript auth types in frontend/src/types/auth.ts
- [ ] T040 [P] Create TypeScript note types in frontend/src/types/note.ts
- [ ] T041 [P] Create auth API service with Axios in frontend/src/services/authApi.ts
- [ ] T042 [P] Create notes API service with Axios in frontend/src/services/notesApi.ts
- [ ] T043 Create Pinia auth store in frontend/src/stores/authStore.ts
- [ ] T044 [P] Create Pinia notes store in frontend/src/stores/notesStore.ts
- [ ] T045 Configure Vue Router with auth guards in frontend/src/router/index.ts
- [ ] T046 Setup App.vue with router-view in frontend/src/App.vue
- [ ] T047 Configure main.ts with Pinia and Router in frontend/src/main.ts
- [ ] T048 [P] Create useAuth composable in frontend/src/composables/useAuth.ts
- [ ] T049 [P] Create LoadingSpinner component in frontend/src/components/common/LoadingSpinner.vue
- [ ] T050 [P] Create ConfirmDialog component in frontend/src/components/common/ConfirmDialog.vue
- [ ] T051 Configure Axios interceptor for JWT token refresh in frontend/src/services/authApi.ts

**Checkpoint**: Foundation ready - authentication implementation can begin

---

## Phase 3: User Story 5 - User Registration (Priority: P5)

**Goal**: Allow new users to register with email and password, receive tokens on success

**Independent Test**: Fill registration form, submit, verify account created and tokens received

### Implementation for User Story 5

- [ ] T052 [US5] Implement POST /api/auth/register endpoint in backend/src/TechbodiaNotes.Api/Controllers/AuthController.cs
- [ ] T053 [US5] Add RegisterAsync method to AuthService in backend/src/TechbodiaNotes.Api/Services/AuthService.cs
- [ ] T054 [US5] Add CreateUserAsync method to UserRepository in backend/src/TechbodiaNotes.Api/Repositories/UserRepository.cs
- [ ] T055 [US5] Add email uniqueness validation in AuthService
- [ ] T056 [US5] Add password hashing with BCrypt in AuthService
- [ ] T057 [P] [US5] Create RegisterForm component in frontend/src/components/auth/RegisterForm.vue
- [ ] T058 [US5] Create RegisterPage in frontend/src/pages/RegisterPage.vue
- [ ] T059 [US5] Add register action to auth store in frontend/src/stores/authStore.ts
- [ ] T060 [US5] Add register API call in frontend/src/services/authApi.ts
- [ ] T061 [US5] Add form validation (email format, password min length) in frontend/src/components/auth/RegisterForm.vue
- [ ] T062 [US5] Handle registration errors (duplicate email) with user feedback

**Checkpoint**: User can register - Registration complete

---

## Phase 4: User Story 6 - User Login (Priority: P6)

**Goal**: Allow registered users to login with email and password, receive access and refresh tokens

**Independent Test**: Enter valid credentials, submit, verify tokens received and redirected to notes

### Implementation for User Story 6

- [ ] T063 [US6] Implement POST /api/auth/login endpoint in backend/src/TechbodiaNotes.Api/Controllers/AuthController.cs
- [ ] T064 [US6] Add LoginAsync method to AuthService in backend/src/TechbodiaNotes.Api/Services/AuthService.cs
- [ ] T065 [US6] Add GetUserByEmailAsync method to UserRepository in backend/src/TechbodiaNotes.Api/Repositories/UserRepository.cs
- [ ] T066 [US6] Add password verification with BCrypt in AuthService
- [ ] T067 [US6] Generate and store refresh token on login in AuthService
- [ ] T068 [P] [US6] Create LoginForm component in frontend/src/components/auth/LoginForm.vue
- [ ] T069 [US6] Create LoginPage in frontend/src/pages/LoginPage.vue
- [ ] T070 [US6] Add login action to auth store in frontend/src/stores/authStore.ts
- [ ] T071 [US6] Add login API call in frontend/src/services/authApi.ts
- [ ] T072 [US6] Store tokens securely in auth store (localStorage with security considerations)
- [ ] T073 [US6] Redirect to notes page after successful login
- [ ] T074 [US6] Handle login errors (invalid credentials) with user feedback

**Checkpoint**: User can login - Login complete

---

## Phase 5: User Story 7 - User Logout & Token Refresh (Priority: P7)

**Goal**: Allow users to logout (revoke tokens) and auto-refresh expired access tokens

**Independent Test**: Click logout, verify tokens cleared and redirected to login

### Implementation for User Story 7

- [ ] T075 [US7] Implement POST /api/auth/refresh endpoint in backend/src/TechbodiaNotes.Api/Controllers/AuthController.cs
- [ ] T076 [US7] Implement POST /api/auth/logout endpoint in backend/src/TechbodiaNotes.Api/Controllers/AuthController.cs
- [ ] T077 [US7] Add RefreshTokenAsync method to AuthService in backend/src/TechbodiaNotes.Api/Services/AuthService.cs
- [ ] T078 [US7] Add LogoutAsync method (revoke refresh token) to AuthService in backend/src/TechbodiaNotes.Api/Services/AuthService.cs
- [ ] T079 [US7] Add token validation and refresh token lookup to RefreshTokenRepository
- [ ] T080 [US7] Add logout action to auth store in frontend/src/stores/authStore.ts
- [ ] T081 [US7] Add logout API call in frontend/src/services/authApi.ts
- [ ] T082 [US7] Implement automatic token refresh in Axios interceptor in frontend/src/services/authApi.ts
- [ ] T083 [US7] Add logout button to App.vue header in frontend/src/App.vue
- [ ] T084 [US7] Clear tokens and redirect to login on logout
- [ ] T085 [US7] Redirect to login when refresh token is invalid/expired

**Checkpoint**: Authentication complete - Ready for Notes CRUD

---

## Phase 6: User Story 1 - Create a New Note (Priority: P1) 🎯 MVP

**Goal**: Allow authenticated users to create notes with title (required) and content (optional)

**Independent Test**: Login, create a note with just a title, verify it appears in the list

### Implementation for User Story 1

- [ ] T086 [US1] Implement POST /api/notes endpoint in backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
- [ ] T087 [US1] Add CreateAsync method to NotesRepository in backend/src/TechbodiaNotes.Api/Repositories/NotesRepository.cs
- [ ] T088 [US1] Add CreateNoteAsync method to NotesService in backend/src/TechbodiaNotes.Api/Services/NotesService.cs
- [ ] T089 [US1] Add JWT [Authorize] attribute to NotesController
- [ ] T090 [US1] Extract UserId from JWT claims in NotesController
- [ ] T091 [P] [US1] Create NoteForm component in frontend/src/components/notes/NoteForm.vue
- [ ] T092 [US1] Add createNote action to Pinia store in frontend/src/stores/notesStore.ts
- [ ] T093 [US1] Add createNote API call in frontend/src/services/notesApi.ts
- [ ] T094 [US1] Implement create note UI in NotesPage with form modal in frontend/src/pages/NotesPage.vue
- [ ] T095 [US1] Add form validation (title required, length limits) in frontend/src/components/notes/NoteForm.vue
- [ ] T096 [US1] Add loading state and success feedback for create operation

**Checkpoint**: User can create notes with validation - MVP functional

---

## Phase 7: User Story 2 - View Notes List and Details (Priority: P2)

**Goal**: Display list of authenticated user's notes, allow clicking to view full details

**Independent Test**: Login, navigate to notes list, verify only your notes display, click to see details

### Implementation for User Story 2

- [ ] T097 [US2] Implement GET /api/notes endpoint with sorting in backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
- [ ] T098 [US2] Implement GET /api/notes/{id} endpoint in backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
- [ ] T099 [US2] Add GetAllByUserIdAsync method with sorting to NotesRepository
- [ ] T100 [US2] Add GetByIdAsync method to NotesRepository in backend/src/TechbodiaNotes.Api/Repositories/NotesRepository.cs
- [ ] T101 [US2] Add GetNotesAsync and GetNoteByIdAsync to NotesService with user filtering
- [ ] T102 [P] [US2] Create NoteCard component in frontend/src/components/notes/NoteCard.vue
- [ ] T103 [P] [US2] Create NoteList component in frontend/src/components/notes/NoteList.vue
- [ ] T104 [P] [US2] Create NoteDetail component in frontend/src/components/notes/NoteDetail.vue
- [ ] T105 [US2] Add fetchNotes and fetchNoteById actions to Pinia store
- [ ] T106 [US2] Add getNotes and getNoteById API calls in frontend/src/services/notesApi.ts
- [ ] T107 [US2] Implement NotesPage with list view in frontend/src/pages/NotesPage.vue
- [ ] T108 [US2] Implement NoteDetailPage with full note display in frontend/src/pages/NoteDetailPage.vue
- [ ] T109 [US2] Add empty state message when no notes exist
- [ ] T110 [US2] Add loading states for list and detail views

**Checkpoint**: User can view notes list and details - Read functionality complete

---

## Phase 8: User Story 3 - Edit an Existing Note (Priority: P3)

**Goal**: Allow users to edit title and content of their own notes, auto-update timestamp

**Independent Test**: Open a note, modify title/content, save, verify changes persist with updated timestamp

### Implementation for User Story 3

- [ ] T111 [US3] Implement PUT /api/notes/{id} endpoint in backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
- [ ] T112 [US3] Add UpdateAsync method to NotesRepository
- [ ] T113 [US3] Add UpdateNoteAsync method to NotesService
- [ ] T114 [US3] Add ownership validation (403 if not owner) in NotesService
- [ ] T115 [US3] Extend NoteForm component to support edit mode in frontend/src/components/notes/NoteForm.vue
- [ ] T116 [US3] Add updateNote action to Pinia store
- [ ] T117 [US3] Add updateNote API call in frontend/src/services/notesApi.ts
- [ ] T118 [US3] Add edit button and form to NoteDetailPage
- [ ] T119 [US3] Add cancel edit functionality
- [ ] T120 [US3] Show updated timestamp after save

**Checkpoint**: User can edit notes - Update functionality complete

---

## Phase 9: User Story 4 - Delete a Note (Priority: P4)

**Goal**: Allow users to delete their own notes with confirmation dialog

**Independent Test**: Select a note, click delete, confirm, verify note removed from list

### Implementation for User Story 4

- [ ] T121 [US4] Implement DELETE /api/notes/{id} endpoint in backend/src/TechbodiaNotes.Api/Controllers/NotesController.cs
- [ ] T122 [US4] Add DeleteAsync method to NotesRepository
- [ ] T123 [US4] Add DeleteNoteAsync method to NotesService
- [ ] T124 [US4] Add ownership validation for delete (403 if not owner) in NotesService
- [ ] T125 [US4] Add deleteNote action to Pinia store
- [ ] T126 [US4] Add deleteNote API call in frontend/src/services/notesApi.ts
- [ ] T127 [US4] Add delete button with confirmation dialog in NoteDetailPage
- [ ] T128 [US4] Handle delete confirmation and cancellation
- [ ] T129 [US4] Navigate back to list after successful delete

**Checkpoint**: Full CRUD complete - Delete functionality complete

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T130 [P] Add search functionality to GET /api/notes in NotesController
- [ ] T131 [P] Create SearchBar component in frontend/src/components/common/SearchBar.vue
- [ ] T132 [P] Create SortDropdown component in frontend/src/components/common/SortDropdown.vue
- [ ] T133 Integrate search and sort in NotesPage
- [ ] T134 [P] Add responsive design classes to all components using TailwindCSS
- [ ] T135 [P] Add keyboard accessibility (focus states, tab navigation) to forms
- [ ] T136 [P] Add error handling and user-friendly error messages
- [ ] T137 [P] Add global error boundary and API error interceptor
- [ ] T138 Run database migrations and seed sample data
- [ ] T139 Validate quickstart.md instructions work end-to-end

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 5 - Register (Phase 3)**: Depends on Foundational phase completion
- **User Story 6 - Login (Phase 4)**: Depends on US5 (needs registration to have users)
- **User Story 7 - Logout/Refresh (Phase 5)**: Depends on US6 (needs login to logout)
- **User Story 1 - Create (Phase 6)**: Depends on US7 (needs authentication complete)
- **User Story 2 - Read (Phase 7)**: Depends on US7 (can parallel with US1)
- **User Story 3 - Update (Phase 8)**: Depends on US2 (needs note detail page)
- **User Story 4 - Delete (Phase 9)**: Depends on US2 (needs note detail page)
- **Polish (Phase 10)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 5 (Register)**: Depends on Foundational - First auth story
- **User Story 6 (Login)**: Depends on US5 - Needs users to exist
- **User Story 7 (Logout/Refresh)**: Depends on US6 - Needs login functionality
- **User Story 1 (Create)**: Depends on US7 - Needs authentication complete
- **User Story 2 (Read)**: Depends on US7 - Can parallel with US1
- **User Story 3 (Update)**: Depends on US2 - Extends NoteDetailPage
- **User Story 4 (Delete)**: Depends on US2 - Extends NoteDetailPage

### Within Each User Story

- Backend endpoint before frontend integration
- Repository before Service before Controller
- Store actions before page components
- Core implementation before validation/error handling

### Parallel Opportunities

- All Setup tasks T003-T008 can run in parallel
- All DTO tasks T016-T023 can run in parallel
- All domain model tasks T013-T015 can run in parallel
- All common components T049-T050 can run in parallel
- Frontend foundation tasks T039-T044 can run in parallel
- US1 and US2 can run in parallel after authentication complete
- US3 and US4 can run in parallel after US2

---

## Implementation Strategy

### Authentication First

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 5 (Register)
4. Complete Phase 4: User Story 6 (Login)
5. Complete Phase 5: User Story 7 (Logout/Refresh)
6. **STOP and VALIDATE**: Test authentication flow works end-to-end

### MVP Second (Notes CRUD)

1. Complete Phase 6: User Story 1 (Create Note)
2. **STOP and VALIDATE**: Test note creation works with authenticated user
3. Complete Phase 7: User Story 2 (Read Notes)
4. Complete Phase 8: User Story 3 (Update Notes)
5. Complete Phase 9: User Story 4 (Delete Notes)
6. Add Phase 10: Polish → Test → Deploy

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US5 (Register) → Test → Users can register
3. Add US6 (Login) → Test → Users can login
4. Add US7 (Logout/Refresh) → Test → Full authentication flow
5. Add US1 (Create) → Test → MVP with notes creation
6. Add US2 (Read) → Test → Users can view notes
7. Add US3 (Update) → Test → Users can edit
8. Add US4 (Delete) → Test → Full CRUD
9. Add Polish → Test → Deploy

### Parallel Team Strategy

With 2+ developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 5 (Register)
   - Developer B: Auth infrastructure (token service, middleware)
3. After auth infrastructure:
   - Developer A: User Story 6 (Login)
   - Developer B: User Story 7 (Logout/Refresh)
4. After auth complete:
   - Developer A: User Story 1 (Create)
   - Developer B: User Story 2 (Read)
5. After US2 complete:
   - Developer A: User Story 3 (Update)
   - Developer B: User Story 4 (Delete)
6. Both work on Polish phase together

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- **Authentication is REQUIRED** - All note endpoints require JWT authentication
- UserId is extracted from JWT claims, never from request body
- Refresh tokens expire after 7 days, access tokens after 15 minutes
