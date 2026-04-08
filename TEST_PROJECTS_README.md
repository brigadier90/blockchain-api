# BlockchainApi Test Projects

Three comprehensive test projects have been created for the BlockchainApi solution:

## 1. **BlockchainApi.UnitTests**
Unit tests for business logic with mocked dependencies (Moq).

### Coverage:
- **Commands/GetBlockCypherCommandHandlerTests.cs**
  - ✓ Handle_WithValidCoin_FetchesAndSaves
  - ✓ Handle_WithInvalidCoin_ReturnsBadRequest
  - ✓ Handle_WhenClientFails_ReturnsServiceUnavailable

- **Queries/GetBlockCypherHistoryQueryHandlerTests.cs**
  - ✓ Handle_WithValidCoin_ReturnsSuccess
  - ✓ Handle_WithInvalidCoin_ReturnsBadRequest
  - ✓ Handle_WithInvalidPaginationParams_ReturnsBadRequest (3 data-driven cases)
  - ✓ Handle_WithPaginationParams_PassesToRepository

### Dependencies:
- xUnit 2.6.6
- Moq 4.20.70
- Microsoft.NET.Test.Sdk 17.9.0

---

## 2. **BlockchainApi.IntegrationTests**
Integration tests for data access layer with real EF Core InMemory database.

### Coverage:
- **Repositories/BlockCypherRepositoryTests.cs**
  - ✓ SaveAsync_PersistsRecord
  - ✓ GetAllAsync_WithoutPagination_ReturnsAllRecords
  - ✓ GetAllAsync_WithPagination_ReturnsPagedRecords
  - ✓ GetAllAsync_ForDifferentCoin_ReturnsEmpty

### Features:
- InMemory SQLite testing for repository isolation
- IAsyncLifetime for setup/teardown
- FakeLogger implementation for testing logging

### Dependencies:
- xUnit 2.6.6
- Microsoft.EntityFrameworkCore.Sqlite 10.0.5
- Microsoft.Extensions.DependencyInjection 10.0.0
- Microsoft.Extensions.Logging 10.0.0

---

## 3. **BlockchainApi.FunctionalTests**
End-to-end functional tests with WebApplicationFactory for real HTTP requests.

### Coverage:
- **HealthCheckTests.cs**
  - ✓ HealthCheck_ReturnsOk

### Utilities:
- **TestServerFixture.cs**: Reusable fixture for test server setup and HttpClient management

### Dependencies:
- xUnit 2.6.6
- Microsoft.AspNetCore.Mvc.Testing 10.0.0

---

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test BlockchainApi.UnitTests
dotnet test BlockchainApi.IntegrationTests
dotnet test BlockchainApi.FunctionalTests

# Run with verbose output
dotnet test --logger "console;verbosity=normal"
```

## Test Results
✅ **9/9 tests passing** (UnitTests)
- All business logic handlers tested with dependency mocking
- Pagination validation covered with Theory/InlineData
- Valid and invalid coin scenarios tested
- Repository calls verified with Moq.Verify

---

## File Structure

```
BlockchainApi.UnitTests/
├── BlockchainApi.UnitTests.csproj
├── Commands/
│   └── GetBlockCypherCommandHandlerTests.cs
└── Queries/
    └── GetBlockCypherHistoryQueryHandlerTests.cs

BlockchainApi.IntegrationTests/
├── BlockchainApi.IntegrationTests.csproj
└── Repositories/
    └── BlockCypherRepositoryTests.cs

BlockchainApi.FunctionalTests/
├── BlockchainApi.FunctionalTests.csproj
├── HealthCheckTests.cs
└── TestServerFixture.cs
```

---

## Design Patterns

### Unit Tests
- **Arrange-Act-Assert** pattern
- **Moq** for dependency mocking
- **Theory** tests for data-driven scenarios
- **Verification** of mock calls with Verify()

### Integration Tests
- **InMemory Database** for isolation without DB setup
- **IAsyncLifetime** for async setup/teardown
- **FakeLogger** to avoid logging framework dependencies

### Functional Tests
- **WebApplicationFactory** for full application startup
- **Test fixtures** for reusable test server setup
- Real HTTP client testing

