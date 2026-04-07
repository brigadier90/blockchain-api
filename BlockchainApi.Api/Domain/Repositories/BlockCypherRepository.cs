using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.Api.Domain.Repositories;

public interface IBlockCypherRepository
{
    public Task SaveAsync(BlockCypher record);
    public Task<List<BlockCypher>> GetAllAsync(string coin, int? page = null, int? pageSize = null);
}
