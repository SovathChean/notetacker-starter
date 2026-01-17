# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj and restore dependencies
COPY src/TechbodiaNotes.Api/TechbodiaNotes.Api.csproj ./TechbodiaNotes.Api/
RUN dotnet restore ./TechbodiaNotes.Api/TechbodiaNotes.Api.csproj

# Copy source code
COPY src/TechbodiaNotes.Api/ ./TechbodiaNotes.Api/

# Build and publish
WORKDIR /src/TechbodiaNotes.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TechbodiaNotes.Api.dll"]
