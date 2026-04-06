using System.Net.Http.Json;
using BlockchainApi.Api.Application.Clients;

namespace BlockchainApi.Api.Infrastructure.Clients;

public class BlockCypherClient : IBlockCypherClient
{
    private readonly HttpClient _httpClient;

    public BlockCypherClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BlockCypherResponse> FetchAsync(string coin, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{coin}/main", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return new BlockCypherResponse
        {
            StatusCode = (int)response.StatusCode,
            Content = content
        };
    }
}
