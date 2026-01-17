<!--
SYNC IMPACT REPORT
==================
Version change: (new) → 1.0.0
Modified principles: N/A (initial constitution)
Added sections:
  - Core Principles (5 principles)
  - Technical Standards
  - Development Workflow
  - Governance
Removed sections: N/A
Templates requiring updates:
  - ✅ .specify/templates/plan-template.md (compatible - uses Constitution Check)
  - ✅ .specify/templates/spec-template.md (compatible - no conflicts)
  - ✅ .specify/templates/tasks-template.md (compatible - supports web app structure)
Follow-up TODOs: None
-->

# Techbodia Notes Constitution

## Core Principles

### I. Data Ownership & Isolation

All operations MUST enforce user-scoped data access. Users can only create, read, update,
and delete their own notes.

- Backend API endpoints MUST validate user ownership before any data operation
- Database queries MUST include user context filtering (WHERE UserId = @CurrentUserId)
- Frontend MUST NOT display or access notes belonging to other users
- Unauthorized access attempts MUST return 403 Forbidden, not 404 Not Found

**Rationale**: Privacy and data integrity are non-negotiable. This principle prevents
data leakage and ensures compliance with user expectations.

### II. Type Safety First

All code MUST leverage strong typing to catch errors at compile time rather than runtime.

- Frontend MUST use TypeScript with strict mode enabled (`"strict": true`)
- Backend MUST use C# nullable reference types and explicit DTOs
- API contracts MUST define explicit request/response models (no dynamic or object types)
- Shared types between frontend and backend MUST be documented in contracts/

**Rationale**: Type safety reduces bugs, improves IDE support, and makes refactoring safer.

### III. Clean Architecture

The system MUST maintain clear separation of concerns with independently deployable layers.

- Frontend (Vue + TailwindCSS) and Backend (ASP.NET Core) MUST be separate projects
- Backend MUST follow layered architecture: Controllers → Services → Repositories
- Data access MUST use Dapper repositories, not inline SQL in controllers or services
- Business logic MUST reside in service classes, not in controllers or UI components
- Each layer MUST depend only on abstractions (interfaces), not implementations

**Rationale**: Clean architecture enables independent testing, deployment, and team scaling.

### IV. Reusable & DRY Code

Code MUST be written for reusability and MUST NOT repeat logic unnecessarily.

- Common utilities MUST be extracted into shared modules/services
- Frontend components MUST be composable and accept props for customization
- Backend services MUST be injectable and single-purpose
- Database queries for common operations MUST be centralized in repository methods
- Copy-pasted code blocks (>5 lines) MUST be refactored into shared functions

**Rationale**: DRY code reduces maintenance burden and ensures consistent behavior.

### V. Responsive & Accessible UI

All user interfaces MUST be responsive and follow accessibility best practices.

- UI MUST be mobile-first using TailwindCSS responsive utilities
- All interactive elements MUST be keyboard accessible
- Form inputs MUST have associated labels and validation feedback
- Loading and error states MUST be clearly communicated to users
- State management MUST provide consistent UX across components

**Rationale**: A responsive, accessible UI serves all users regardless of device or ability.

## Technical Standards

This section defines the technology stack and coding standards for the project.

**Frontend Stack**:
- Vue 3 with Composition API and `<script setup>` syntax
- TypeScript with strict mode
- TailwindCSS for styling (utility-first approach)
- Pinia or Vue's built-in reactivity for state management
- Axios or Fetch for API communication

**Backend Stack**:
- C# with ASP.NET Core Web API
- Dapper for data access (lightweight ORM)
- SQL Server database
- Dependency injection for all services

**Code Style**:
- Frontend: ESLint + Prettier with Vue recommended rules
- Backend: .NET code style conventions with nullable enabled
- Meaningful names over comments; self-documenting code preferred

## Development Workflow

This section defines the development process and quality gates.

**Branch Strategy**:
- Feature branches for all changes (e.g., `feature/###-feature-name`)
- Main branch protected; requires passing checks before merge

**API-First Development**:
- Define API contracts before implementation
- Frontend MAY use mock data while backend is in progress
- Contract changes MUST be coordinated between frontend and backend

**Quality Gates**:
- All code MUST pass linting before commit
- All API endpoints MUST be tested (unit or integration)
- Breaking changes MUST be documented in PR description

**Authentication** (Optional for junior position):
- When implemented, MUST use JWT or session-based auth
- All protected endpoints MUST validate authentication
- Unauthorized requests MUST return 401, forbidden MUST return 403

## Governance

This constitution establishes the guiding principles for the Techbodia Notes project.
All development decisions, code reviews, and architectural choices MUST align with
these principles.

**Amendment Process**:
1. Propose changes via pull request to this file
2. Document rationale for principle additions, modifications, or removals
3. Version bump according to semantic versioning rules
4. Update dependent templates if principle-driven sections change

**Compliance**:
- All PRs MUST be reviewed against constitution principles
- Violations MUST be justified in the Complexity Tracking section of plan.md
- Constitution Check in plan-template.md MUST be completed before implementation

**Guidance**: Use `/specs/[feature]/plan.md` for feature-specific implementation guidance.

**Version**: 1.0.0 | **Ratified**: 2026-01-17 | **Last Amended**: 2026-01-17
