# BlockchainApi

A minimal ASP.NET Core API for querying blockchain data from BlockCypher with built-in persistence and scaling patterns.

## Quick Start

### Prerequisites
- .NET 10.0 SDK
- SQLite (included with project)

### Run

```bash
# Build
dotnet build

# Run
dotnet run --project BlockchainApi.Api

# Tests
dotnet test
```

Server starts on `http://127.0.0.1:5284`
Swagger UI: `http://127.0.0.1:5284/swagger/`

## API Endpoints

### Get Latest Block
```
GET /api/blockcypher/v1/{coin}/main
```
Fetches and persists the latest block data from BlockCypher.

**Coins:** btc, eth, ltc, dash, doge

### Get History
```
GET /api/blockcypher/v1/{coin}/history?page=1&pageSize=20
```
Returns persisted snapshots. Pagination optional (returns all if omitted).

### Health
```
GET /health
```

## How It Works

**Architecture:**
- **CQRS Pattern** - Separate command (write) and query (read) handlers for scalability
- **MediatR** - Command/query dispatch with structured logging
- **Repository Pattern** - Data access abstraction with async methods
- **SQLite + EF Core** - Local persistence with migration support

**Scaling Features:**
- **Pagination** - Database-level Skip/Take for large result sets
- **Indices** - Composite index on (Coin, CreatedAt DESC) for query performance
- **Async/Await** - Full async pipeline from controller to database
- **Structured Logging** - Semantic logging across all layers for observability

## Project Structure

```
BlockchainApi.Api/              # Main API
├── Application/                # Business logic
│   ├── Commands/              # Write operations
│   └── Queries/               # Read operations
├── Domain/                     # Core models & interfaces
├── Infrastructure/            # EF Core, HTTP client, repositories
└── Controllers/               # API endpoints

BlockchainApi.UnitTests/        # Handler tests with Moq
BlockchainApi.IntegrationTests/ # Repository tests with InMemory DB
BlockchainApi.FunctionalTests/  # End-to-end API tests
```

## Configuration

Default pagination: 20 records per page, optional query params.

Database location: `blockcypher.db` (auto-created on first run)

## Development

Watch mode:
```bash
dotnet watch run --project BlockchainApi.Api
```

Swagger UI: `http://127.0.0.1:5284/swagger/`

## EF Core Migrations (SQLite)

This project uses EF Core migrations for schema management.

### One-time setup
```bash
dotnet tool install --global dotnet-ef --version 10.0.5
```

### Recreate exactly what was done in this repo
```bash
# 1) Create the initial migration (already committed in this repo)
dotnet ef migrations add InitialCreate \
	--project BlockchainApi.Api/BlockchainApi.Api.csproj \
	--startup-project BlockchainApi.Api/BlockchainApi.Api.csproj \
	--context BlockchainApi.Api.Infrastructure.Persistence.BlockCypherContext \
	--output-dir Infrastructure/Persistence/Migrations

# 2) Apply migrations to SQLite database
dotnet ef database update \
	--project BlockchainApi.Api/BlockchainApi.Api.csproj \
	--startup-project BlockchainApi.Api/BlockchainApi.Api.csproj \
	--context BlockchainApi.Api.Infrastructure.Persistence.BlockCypherContext
```

### Notes
- Database file is `blockcypher.db` at the repository root.
- App startup runs `dbContext.Database.Migrate()` so schema is applied automatically.
