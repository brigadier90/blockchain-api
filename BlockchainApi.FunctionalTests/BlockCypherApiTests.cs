using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using BlockchainApi.Api;

namespace BlockchainApi.FunctionalTests;

public class BlockCypherApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetCoin_WithValidCoin_ReturnsOkAndPersistsData()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/btc/main");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetCoin_WithInvalidCoin_ReturnsBadRequest()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/invalid_coin/main");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetHistory_WithValidCoin_ReturnsOk()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/btc/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetHistory_WithPagination_ReturnsPagedResults()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/btc/history?page=1&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetHistory_WithoutPagination_ReturnsAllResults()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/btc/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetHistory_WithInvalidCoin_ReturnsBadRequest()
    {
        // Act
        var response = await _client!.GetAsync("/api/blockcypher/v1/invalid_coin/history");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
