using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BlockchainApi.FunctionalTests;

public class TestServerFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    public HttpClient? Client { get; private set; }

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        Client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }
}
