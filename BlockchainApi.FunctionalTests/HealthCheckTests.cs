using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using BlockchainApi.Api;

namespace BlockchainApi.FunctionalTests;

public class HealthCheckTests : IAsyncLifetime
{
    private BlockchainApiApplication? _app;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _app = new BlockchainApiApplication();
        _client = _app.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _app?.Dispose();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client!.GetAsync("/health");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

/// <summary>
/// WebApplicationFactory for setting up the test server.
/// For functional testing with real HTTP requests.
/// </summary>
public class BlockchainApiApplication : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public BlockchainApiApplication()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    public HttpClient CreateClient() => _factory.CreateClient();

    public void Dispose() => _factory.Dispose();
}
