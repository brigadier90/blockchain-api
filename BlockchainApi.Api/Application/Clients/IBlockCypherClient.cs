namespace BlockchainApi.Api.Application.Clients;

public interface IBlockCypherClient
{
    Task<BlockCypherResponse> FetchAsync(string coin, CancellationToken cancellationToken = default);
}
