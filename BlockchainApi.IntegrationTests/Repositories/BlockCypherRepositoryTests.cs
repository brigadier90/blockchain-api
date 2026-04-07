using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BlockchainApi.Api.Domain.Models;
using BlockchainApi.Api.Infrastructure.Persistence;
using BlockchainApi.Api.Infrastructure.Repositories;

namespace BlockchainApi.IntegrationTests.Repositories;

public class BlockCypherRepositoryTests : IAsyncLifetime
{
    private readonly BlockCypherContext _context;
    private readonly BlockCypherRepository _repository;

    public BlockCypherRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BlockCypherContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BlockCypherContext(options);
        var logger = new FakeLogger<BlockCypherRepository>();
        _repository = new BlockCypherRepository(_context, logger);
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task SaveAsync_PersistsRecord()
    {
        // Arrange
        var record = new BlockCypher 
        { 
            Coin = "btc", 
            RawData = "{}", 
            CreatedAt = DateTime.UtcNow 
        };

        // Act
        await _repository.SaveAsync(record);
        var saved = await _context.BlockCyphers.FirstOrDefaultAsync(x => x.Coin == "btc");

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("btc", saved.Coin);
    }

    [Fact]
    public async Task GetAllAsync_WithoutPagination_ReturnsAllRecords()
    {
        // Arrange
        var records = new[]
        {
            new BlockCypher { Coin = "btc", RawData = "{}", CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
            new BlockCypher { Coin = "btc", RawData = "{}", CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
            new BlockCypher { Coin = "btc", RawData = "{}", CreatedAt = DateTime.UtcNow }
        };
        _context.BlockCyphers.AddRange(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync("btc");

        // Assert
        Assert.Equal(3, result.Count);
        // Should be descending by CreatedAt
        Assert.True(result[0].CreatedAt > result[1].CreatedAt);
        Assert.True(result[1].CreatedAt > result[2].CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsPagedRecords()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            var record = new BlockCypher 
            { 
                Coin = "btc", 
                RawData = "{}", 
                CreatedAt = DateTime.UtcNow.AddMinutes(-i) 
            };
            _context.BlockCyphers.Add(record);
        }
        await _context.SaveChangesAsync();

        // Act
        var page1 = await _repository.GetAllAsync("btc", 1, 10);
        var page2 = await _repository.GetAllAsync("btc", 2, 10);

        // Assert
        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
        Assert.NotEqual(page1.First().Id, page2.First().Id);
    }

    [Fact]
    public async Task GetAllAsync_ForDifferentCoin_ReturnsEmpty()
    {
        // Arrange
        var record = new BlockCypher { Coin = "btc", RawData = "{}", CreatedAt = DateTime.UtcNow };
        _context.BlockCyphers.Add(record);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync("eth");

        // Assert
        Assert.Empty(result);
    }
}

public class FakeLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}
