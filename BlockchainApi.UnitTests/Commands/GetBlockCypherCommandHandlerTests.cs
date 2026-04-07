using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using BlockchainApi.Api.Application.Commands;
using BlockchainApi.Api.Application.Clients;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.UnitTests.Commands;

public class GetBlockCypherCommandHandlerTests
{
    private readonly Mock<IBlockCypherClient> _mockClient;
    private readonly Mock<IBlockCypherRepository> _mockRepository;
    private readonly Mock<ILogger<GetBlockCypherCommandHandler>> _mockLogger;
    private readonly GetBlockCypherCommandHandler _handler;

    public GetBlockCypherCommandHandlerTests()
    {
        _mockClient = new Mock<IBlockCypherClient>();
        _mockRepository = new Mock<IBlockCypherRepository>();
        _mockLogger = new Mock<ILogger<GetBlockCypherCommandHandler>>();
        _handler = new GetBlockCypherCommandHandler(_mockClient.Object, _mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidCoin_FetchesAndSaves()
    {
        // Arrange
        var coin = "btc";
        var rawData = @"{""name"":""Bitcoin""}";
        var mockResponse = new BlockCypherResponse 
        { 
            StatusCode = 200, 
            Content = rawData 
        };
        
        _mockClient.Setup(c => c.FetchAsync(coin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);
        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<BlockCypher>()))
            .Returns(Task.CompletedTask);

        var command = new GetBlockCypherCommand(coin);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _mockClient.Verify(c => c.FetchAsync(coin, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<BlockCypher>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCoin_ReturnsBadRequest()
    {
        // Arrange
        var coin = "invalid";
        var command = new GetBlockCypherCommand(coin);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
        _mockClient.Verify(c => c.FetchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<BlockCypher>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenClientFails_ReturnsServiceUnavailable()
    {
        // Arrange
        var coin = "btc";
        var mockResponse = new BlockCypherResponse 
        { 
            StatusCode = 503, 
            Content = "Service unavailable" 
        };
        _mockClient.Setup(c => c.FetchAsync(coin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var command = new GetBlockCypherCommand(coin);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(503, result.StatusCode);
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<BlockCypher>()), Times.Never);
    }
}

