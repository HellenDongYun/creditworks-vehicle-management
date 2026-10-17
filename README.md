
# CreditWorks Vehicle Management

A full-stack vehicle management application developed for the CreditWorks Software Engineer technical assignment. It allows users to register vehicles, sort and view vehicle records, and manage configurable weight-based vehicle categories.

## Features

- Register vehicles with owner name, manufacturer, manufacture year, and weight in kilograms.
- View vehicles with their manufacturer, weight, and calculated category.
- Sort vehicles by owner name, manufacturer, manufacture year, or weight in ascending or descending order.
- Automatically determine a vehicle's category from its weight.
- Create, edit, and delete categories, including their names, icons, and weight boundaries.
- Validate category ranges to prevent gaps, overlaps, and uncovered weights.
- Reflect updated category definitions when displaying existing vehicles.
- Display each vehicle's category icon dynamically using the category's saved icon identifier and Lucide SVG icons.
- Preview category icons while editing category configurations, with a fallback icon for unsupported identifiers.

## Technology Stack

| Layer | Technology |
| --- | --- |
| Frontend | React, TypeScript, Vite, Lucide React |
| Backend | C#, ASP.NET Core (.NET 8) |
| Data access | Entity Framework Core 8 |
| Database | Microsoft SQL Server 2022 (Docker) |
| Tests | xUnit, SQL Server Testcontainers |

## Architecture

The backend is organised into separate projects:

```text
CreditWorks/
├── backend/
│   ├── CreditWorks.sln
│   ├── CreditWorks.Api/
│   ├── CreditWorks.Application/
│   ├── CreditWorks.Domain/
│   ├── CreditWorks.Infrastructure/
│   └── tests/
│       ├── CreditWorks.Application.Tests/
│       ├── CreditWorks.Domain.Tests/
│       └── CreditWorks.IntegrationTests/
├── frontend/
├── compose.yaml
├── .env.example
└── README.md
```

- **API:** HTTP endpoints, request handling, and application configuration.
- **Application:** Use cases, DTOs, validation coordination, and service interfaces.
- **Domain:** Core business rules and domain-specific exceptions.
- **Infrastructure:** Entity Framework Core persistence, repositories, and SQL Server integration.
- **Frontend:** React components, typed API clients, vehicle registration, list sorting, and category administration.

This separation keeps business rules independent from presentation and database implementation details, and makes application logic easier to test.

## Prerequisites

Install:

- .NET 8 SDK
- Node.js and npm (a current LTS release is recommended)
- Docker Desktop with Docker Compose
- Git

Docker must be running before starting SQL Server or running database integration tests.

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/HellenDongYun/creditworks-vehicle-management.git
cd creditworks-vehicle-management
```

### 2. Configure local environment

```bash
cp .env.example .env
```

The supplied `.env.example` defines `MSSQL_SA_PASSWORD`. Replace its example value in `.env` with a strong local password that satisfies SQL Server password requirements. Docker Compose reads this value and passes it to the SQL Server container. Do not commit `.env` or real credentials.

The API reads `ConnectionStrings:DefaultConnection` from ASP.NET Core configuration. It must point to the Docker SQL Server instance and use **the same SA password** as `.env`. One option is an environment variable set in the terminal that will run both the EF migration command and the API:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=CreditWorks;User Id=sa;Password=YOUR_LOCAL_PASSWORD;Encrypt=True;TrustServerCertificate=True'
```

Replace `YOUR_LOCAL_PASSWORD` locally. Do not commit a password or a real connection string. Environment variables set in one terminal are not automatically available in another; repeat the export where necessary. If the repository already provides an untracked local configuration mechanism, it can be used instead. ASP.NET Core user secrets are another option **only if** the API project has a configured `UserSecretsId`.

The SQL Server container is based on SQL Server 2022 Developer Edition, publishes `1433` only to `127.0.0.1`, and uses a named Docker volume (`sqlserver_data`) for database files. On Apple Silicon, Compose requests the `linux/amd64` image platform.

### 3. Start SQL Server

```bash
docker compose up -d
docker compose ps
```

Wait until SQL Server is ready to accept connections. If startup fails, inspect the container logs:

```bash
docker compose logs sqlserver
```

### 4. Restore and build the backend

```bash
cd backend
dotnet restore
dotnet build
```

### 5. Initialise the database

This repository includes EF Core migrations under `backend/CreditWorks.Infrastructure/Persistence/Migrations`. The database schema must be created **before** starting the API, because the startup seeder queries the manufacturer and category tables.

From the **repository root**, restore the local .NET tools and apply the existing database migrations:

```bash
dotnet tool restore

dotnet ef database update \
  --project backend/CreditWorks.Infrastructure \
  --startup-project backend/CreditWorks.Api
```

The repository includes a local .NET tool manifest (`.config/dotnet-tools.json`) that pins the Entity Framework Core CLI (`dotnet-ef`) to version `8.0.32`. Running `dotnet tool restore` installs the required tool locally, so reviewers do not need to install it globally.

Before applying migrations, ensure SQL Server is running and `ConnectionStrings__DefaultConnection` (or an equivalent configuration source) is correctly configured.

The EF Core migration workflow and API startup have been successfully exercised during local development. `Program.cs` calls `DatabaseSeeder.SeedAsync(db, seedDemoVehicles: app.Environment.IsDevelopment())` on startup.

The seeder **does not create tables or apply migrations**. It initializes reference data when the corresponding tables are empty. In the Development environment, it also inserts eight sample vehicles if the Vehicles table is empty. Existing vehicle records are preserved, and demo vehicle seeding is disabled outside the Development environment.

**Initial manufacturers:** Mazda, Mercedes, Honda, Ferrari, Toyota.

**Initial categories:**

| Category | Icon identifier | Weight range (kg) |
| --- | --- | --- |
| Light | `car-front` | `[0, 500)` |
| Medium | `car` | `[500, 2500)` |
| Heavy | `truck` | `[2500, ∞)` |

The minimum boundary is inclusive, and a finite maximum is exclusive. The seeder does not overwrite existing manufacturer or category records when their respective tables contain data. The Docker named volume `sqlserver_data` retains database files across ordinary container restarts.

#### Development Demo Vehicle Data

In the Development environment, the application is configured to seed **eight sample vehicles**, but **only when the `Vehicles` table is empty**. The examples cover multiple manufacturers, manufacture years, and category boundaries, including exactly `500 kg` and `2500 kg`.

If the database already contains vehicles, demo vehicle insertion is skipped and all existing records are preserved. For example, a local database containing five previously created vehicles will continue to show those five vehicles after startup; it will not automatically increase to eight or thirteen.

Demo vehicle seeding is disabled outside the Development environment. Restarting the application does not add duplicate demo vehicles while the table contains records.

**Note:** The eight-vehicle seeding behaviour is configured in code; verification against a fresh, empty database is still pending.


### 6. Run the backend

From `backend/`:

```bash
dotnet run --project CreditWorks.Api
```

During local development, the API has been accessed at:

- API: `http://localhost:5048`
- Swagger: `http://localhost:5048/swagger`

The actual URL is the one printed by `dotnet run`. Swagger is enabled only when `ASPNETCORE_ENVIRONMENT=Development`. The API also configures global exception handling with Problem Details.

### 7. Run the frontend

In another terminal, from the repository root:

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` (or the URL printed by Vite).

The frontend uses an Axios client with `baseURL: '/api'`. The Vite development server proxies `/api` requests to `http://localhost:5048` with `changeOrigin: true`, so the backend must listen on that port for local development. This setup avoids cross-origin browser requests and does not require CORS middleware during development. The Vite proxy is a development-server feature; a production deployment would need its own reverse-proxy or API URL strategy.

## Application Usage

### Vehicles

Use **Add Vehicle** to register a vehicle. The application determines its category from its weight. The vehicle list supports sorting by owner name, manufacturer, manufacture year, and weight. Each vehicle displays the icon assigned to its current category.

### Categories

Use **Categories** to edit category names, icon identifiers (with live preview), and weight boundaries, add new categories, or remove existing ones. Click **Save Changes** to persist a valid configuration. After a configuration change, vehicles are classified according to the current ranges.

Category icon identifiers are stored as strings in SQL Server and returned with vehicle data as `categoryIcon`. The shared `CategoryIcon.tsx` component maps supported identifiers such as `car-front`, `car`, and `truck` to Lucide SVG icons. The vehicle list renders the saved icon dynamically, and the category editor shows a live icon preview. Unsupported identifiers display a fallback icon. **Icon editing, saving, and dynamic vehicle-list rendering have been manually tested successfully.**

## Category Rules and Boundary Semantics

The seeded default categories are:

| Category | Weight range (kg) |
| --- | --- |
| Light | `0 ≤ weight < 500` |
| Medium | `500 ≤ weight < 2500` |
| Heavy | `weight ≥ 2500` |

Categories are configurable. The system enforces the following rules:

1. The first category starts at **0 kg**.
2. Each range has an **inclusive lower bound** and, except for the last category, an **exclusive upper bound**.
3. Adjacent categories meet exactly: the previous maximum equals the next minimum.
4. Ranges cannot overlap or leave gaps.
5. Only the final category has no upper weight limit.
6. Category names must be unique.
7. Vehicle weight must be positive and support up to two decimal places.

For example, a vehicle weighing exactly `500.00 kg` belongs to **Medium**, not Light. A vehicle weighing exactly `2500.00 kg` belongs to **Heavy**.

## Data Model and Design Decisions

### Manufacturers

Manufacturers are represented separately from vehicle records rather than repeated as hard-coded frontend values. The initial manufacturer set is Mazda, Mercedes, Honda, Ferrari, and Toyota.

### Categories

A category has an identifier, name, icon identifier, minimum weight, and optional maximum weight. The optional maximum represents an open-ended final range. Icon identifiers are stored as strings and rendered using the frontend icon mapping.

### Category assignment

A vehicle's category is determined from its weight and the **current** category configuration. This prevents the category shown for an existing vehicle from becoming stale after weight boundaries change.

### Category configuration updates

The backend validates the full proposed category configuration before accepting it. The repository performs category configuration updates transactionally, helping prevent partial changes and preserve consistency. Category edits, additions, and removals are handled together as a single configuration update.

## API Overview

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `GET` | `/api/vehicles` | List vehicles, with sorting parameters |
| `GET` | `/api/vehicles/{id}` | Get a vehicle |
| `POST` | `/api/vehicles` | Register a vehicle |
| `GET` | `/api/manufacturers` | List manufacturers |
| `GET` | `/api/categories` | List categories |
| `PUT` | `/api/categories` | Replace category configuration |
| `POST` | `/api/categories` | Create a category |
| `PUT` | `/api/categories/{id}` | Update a category |
| `DELETE` | `/api/categories/{id}` | Delete a category |

Refer to Swagger for request/response schemas and the exact supported sorting query parameters.

## Automated Testing

From the repository root:

```bash
dotnet test backend/CreditWorks.sln
```

To run the category integration tests separately from the repository root:

```bash
dotnet test backend/tests/CreditWorks.IntegrationTests/CreditWorks.IntegrationTests.csproj \
  --filter "FullyQualifiedName~CategoryRepositoryTests"
```

The test suite includes application-level category validation tests, domain tests, and SQL Server integration tests. Category tests cover valid configurations, gaps, overlaps, duplicate names, invalid boundaries, repository persistence, and rollback behaviour. Integration tests use Testcontainers and require a working Docker daemon.

For frontend checks, from the repository root:

```bash
cd frontend
npm install
npm run build
npm run lint
```

## Assumptions and Trade-offs

- Categories cover non-negative weight values, while an individual vehicle must have a strictly positive weight.
- A range's lower bound is inclusive and its upper bound is exclusive, avoiding ambiguous assignments at shared boundaries.
- The last category has no maximum weight, allowing future heavier vehicles without reconfiguration.
- The category management UI submits the complete configuration so validation can consider all ranges together.
- A limited icon-name mapping keeps the UI simple and predictable, but does not support every Lucide icon without adding it to the mapping.
- The application is intended as a technical exercise, not a production-ready multi-user service.
- SQL Server Developer Edition is for development/testing rather than production deployment.
- The frontend Vite proxy works in local development; production hosting would require separate API routing configuration.

## Known Limitations and Possible Improvements

Potential future enhancements include:

- Authentication and role-based access for category administration.
- Concurrency/version checks for simultaneous category edits by multiple administrators.
- Pagination and server-side filtering for larger vehicle datasets.
- A searchable icon picker with validation against supported icon names.
- More extensive frontend component and end-to-end tests.
- Structured observability, deployment automation, and production secret management.

## Manual Verification

The following workflow has been manually verified in the running application:

1. Open **Categories** and change an existing category icon identifier to a supported value such as `truck`.
2. Confirm the icon preview updates and save the configuration.
3. Open **Vehicles** and confirm vehicles in that category display the newly selected SVG icon.
4. Restore the original icon, save again, and confirm the vehicle list reflects the restored icon.

Category range edits and validation were also exercised during development, including add/remove operations and rejection of gaps and overlaps.

## Submission

Repository: https://github.com/HellenDongYun/creditworks-vehicle-management

This repository is intended to include the source code, solution/project files, tests, database setup, and documentation necessary to review, build, and run the application.
