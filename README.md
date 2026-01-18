# Techbodia Notes

A full-stack notes application built with ASP.NET Core 8 and Vue 3.

## Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server 2022
- JWT Authentication with token blacklisting

### Frontend
- Vue 3 with Composition API
- TypeScript
- Tailwind CSS
- Vite

## Features

- User registration and authentication
- Login with email or username
- JWT access tokens with refresh token rotation
- Secure logout (token blacklisting)
- Create, read, update, delete notes
- Pagination and search for notes

## Prerequisites

- [Docker](https://www.docker.com/get-started) and Docker Compose

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd techbodia-note
   ```

2. **Start all services**
   ```bash
   docker compose up -d
   ```

3. **Initialize the database** (first time only)
   ```bash
   docker exec -i techbodia-notes-db /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P "YourStrong@Passw0rd" -C \
     -i /dev/stdin < database/docker-init.sql
   ```

4. **Access the application**
   - Frontend: http://localhost:7001
   - Backend API: http://localhost:5001
   - Swagger UI: http://localhost:5001/swagger (development only)

## Services

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 7001 | Vue 3 SPA |
| Backend | 5001 | ASP.NET Core API |
| Database | 1433 | SQL Server 2022 |

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login (email or username) |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (invalidates token) |
| GET | `/api/auth/me` | Get current user |

### Notes
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notes` | List notes (paginated) |
| GET | `/api/notes/{id}` | Get note by ID |
| POST | `/api/notes` | Create note |
| PUT | `/api/notes/{id}` | Update note |
| DELETE | `/api/notes/{id}` | Delete note |

## Development

### Rebuild services after code changes
```bash
# Rebuild and restart backend
docker compose build backend && docker compose up -d backend

# Rebuild and restart frontend
docker compose build frontend && docker compose up -d frontend

# Rebuild all
docker compose build && docker compose up -d
```

### View logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f backend
```

### Reset database
```bash
# Stop and remove volumes
docker compose down -v

# Start fresh
docker compose up -d

# Re-initialize database
docker exec -i techbodia-notes-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -C \
  -i /dev/stdin < database/docker-init.sql
```

### Stop all services
```bash
docker compose down
```

## Environment Variables

### Backend (configured in docker-compose.yml)
| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `JwtSettings__SecretKey` | JWT signing key |
| `JwtSettings__Issuer` | JWT issuer |
| `JwtSettings__Audience` | JWT audience |
| `JwtSettings__AccessTokenExpirationMinutes` | Access token lifetime |
| `JwtSettings__RefreshTokenExpirationDays` | Refresh token lifetime |

## Project Structure

```
techbodia-note/
├── backend/
│   └── src/TechbodiaNotes.Api/
│       ├── Controllers/       # API endpoints
│       ├── DTOs/              # Data transfer objects
│       ├── Infrastructure/    # DbContext
│       ├── Models/            # Entity models
│       ├── Repositories/      # Data access layer
│       └── Services/          # Business logic
├── frontend/
│   └── src/
│       ├── components/        # Vue components
│       ├── composables/       # Vue composables
│       ├── pages/             # Page components
│       ├── services/          # API services
│       ├── stores/            # Pinia stores
│       └── types/             # TypeScript types
├── database/
│   └── docker-init.sql        # Database schema
└── docker-compose.yml
```

## License

MIT
