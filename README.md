# CreditWorks Vehicle Management System

A full-stack web application developed for the CreditWorks Software Engineer technical assignment.

## Technology Stack

- Backend: C# / ASP.NET Core 8
- Frontend: React + TypeScript (planned)
- Database: Microsoft SQL Server 2022
- ORM: Entity Framework Core
- Database Hosting: Docker
- Testing: xUnit

## Architecture

The backend follows a layered architecture:

- **API:** HTTP endpoints, request validation and error handling.
- **Application:** Application services and business workflows.
- **Domain:** Vehicle entities, category ranges and business rules.
- **Infrastructure:** Database persistence and EF Core migrations.

## Current Progress

- [x] Backend solution created
- [x] Layered project structure
- [x] Unit and integration test projects created
- [x] SQL Server Docker environment configured
- [ ] Domain business rules
- [ ] Database entities and migrations
- [ ] Vehicle management API
- [ ] Category management API
- [ ] Automated business-rule tests
- [ ] React frontend

## Development Setup

### Requirements

- .NET 8 SDK
- Docker Desktop
- Git

### Start SQL Server

```bash
cp .env.example .env
docker compose up -d
```

Edit `.env` to use a strong password before starting the database.

### Build Backend

```bash
cd backend
dotnet restore
dotnet build
```

### Run Tests

```bash
dotnet test
```

## Notes

SQL Server 2022 runs as an AMD64 container. Running this image on Apple Silicon requires emulation and is not an officially supported SQL Server configuration.

Additional setup instructions and architectural decisions will be documented as development progresses.