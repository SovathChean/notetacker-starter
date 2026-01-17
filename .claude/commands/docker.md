# /docker - Docker Containerization Skill

Generate Docker configurations for techbodia-note application.

## Usage

```
/docker                     # Generate all Docker files
/docker frontend            # Frontend Dockerfile only
/docker backend             # Backend Dockerfile only
/docker compose             # docker-compose.yml only
/docker prod                # Production-optimized configs
```

## Arguments

| Arg | Description |
|-----|-------------|
| `frontend` | Vue 3 + Nginx Dockerfile |
| `backend` | ASP.NET Core 8 Dockerfile |
| `compose` | Multi-service docker-compose |
| `prod` | Production builds with optimization |
| `dev` | Development with hot reload |

## Output Files

```
├── frontend/
│   └── Dockerfile
├── backend/
│   └── Dockerfile
├── docker-compose.yml
├── docker-compose.prod.yml
└── .dockerignore
```

---

## Templates

### Frontend Dockerfile (Vue 3 + Nginx)

```dockerfile
# frontend/Dockerfile

# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm ci

# Build application
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Frontend Nginx Config

```nginx
# frontend/nginx.conf
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    # Vue Router history mode
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy
    location /api {
        proxy_pass http://backend:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml;
}
```

### Backend Dockerfile (ASP.NET Core 8)

```dockerfile
# backend/Dockerfile

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY src/TechbodiaNotes.Api/*.csproj ./TechbodiaNotes.Api/
RUN dotnet restore TechbodiaNotes.Api/TechbodiaNotes.Api.csproj

# Copy source and build
COPY src/TechbodiaNotes.Api/. ./TechbodiaNotes.Api/
WORKDIR /src/TechbodiaNotes.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Non-root user for security
RUN adduser --disabled-password --gecos '' appuser
USER appuser

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TechbodiaNotes.Api.dll"]
```

### Docker Compose (Development)

```yaml
# docker-compose.yml
version: '3.8'

services:
  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - backend
    networks:
      - app-network

  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=TechbodiaNotesDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
    depends_on:
      db:
        condition: service_healthy
    networks:
      - app-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrong@Passw0rd
    volumes:
      - sqlserver-data:/var/opt/mssql
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1"
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - app-network

networks:
  app-network:
    driver: bridge

volumes:
  sqlserver-data:
```

### Docker Compose (Production)

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  frontend:
    image: techbodia-note-frontend:${TAG:-latest}
    build:
      context: ./frontend
      dockerfile: Dockerfile
      target: production
    ports:
      - "80:80"
    restart: unless-stopped
    depends_on:
      - backend

  backend:
    image: techbodia-note-backend:${TAG:-latest}
    build:
      context: ./backend
      dockerfile: Dockerfile
      target: runtime
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
      - Jwt__Secret=${JWT_SECRET}
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=${DB_PASSWORD}
    volumes:
      - sqlserver-data:/var/opt/mssql
    restart: unless-stopped

volumes:
  sqlserver-data:
```

### .dockerignore

```
# .dockerignore
**/node_modules
**/bin
**/obj
**/.git
**/.gitignore
**/.env
**/.env.*
**/Dockerfile*
**/docker-compose*
**/*.md
**/.vs
**/.vscode
**/coverage
**/dist
```

---

## Commands

```bash
# Build all services
docker-compose build

# Start development
docker-compose up -d

# View logs
docker-compose logs -f backend

# Stop all
docker-compose down

# Production build
docker-compose -f docker-compose.prod.yml build

# Production deploy
docker-compose -f docker-compose.prod.yml up -d
```

## Workflow

1. Analyze project structure
2. Generate appropriate Dockerfile(s)
3. Create docker-compose.yml
4. Create .dockerignore
5. Validate with `docker-compose config`
6. Test build with `docker-compose build`
