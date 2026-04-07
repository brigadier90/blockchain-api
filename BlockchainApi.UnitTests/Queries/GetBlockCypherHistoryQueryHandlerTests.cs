using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using BlockchainApi.Api.Application.Queries;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Dtos;
using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.UnitTests.Queries;

public class GetBlockCypherHistoryQueryHandlerTests
{
    private readonly Mock<IBlockCypherRepository> _mockRepository;
    private readonly Mock<ILogger<GetBlockCypherHistoryQueryHandler>> _mockLogger;
    private readonly GetBlockCypherHistoryQueryHandler _handler;

    public GetBlockCypherHistoryQueryHandlerTests()
    {
        _mockRepository = new Mock<IBlockCypherRepository>();
        _mockLogger = new Mock<ILogger<GetBlockCypherHistoryQueryHandler>>();
        _handler = new GetBlockCypherHistoryQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidCoin_ReturnsSuccess()
    {
        // Arrange
        var coin = "btc";
        var mockData = new List<BlockCypher>
        {
            new() { Id = 1, Coin = coin, RawData = "{}", CreatedAt = DateTime.UtcNow }
        };
        _mockRepository.Setup(r => r.GetAllAsync(coin, null, null))
            .ReturnsAsync(mockData);

        var query = new GetBlockCypherHistoryQuery(coin);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        _mockRepository.Verify(r => r.GetAllAsync(coin, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCoin_ReturnsBadRequest()
    {
        // Arrange
        var coin = "invalid";
        var query = new GetBlockCypherHistoryQuery(coin);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(-1, 20)]
    public async Task Handle_WithInvalidPaginationParams_ReturnsBadRequest(int? page, int? pageSize)
    {
        // Arrange
        var coin = "btc";
        var query = new GetBlockCypherHistoryQuery(coin, page, pageSize);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Handle_WithPaginationParams_PassesToRepository()
    {
        // Arrange
        var coin = "btc";
        var page = 2;
        var pageSize = 10;
        var mockData = new List<BlockCypher>();
        _mockRepository.Setup(r => r.GetAllAsync(coin, page, pageSize))
            .ReturnsAsync(mockData);

        var query = new GetBlockCypherHistoryQuery(coin, page, pageSize);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.GetAllAsync(coin, page, pageSize), Times.Once);
    }
}
