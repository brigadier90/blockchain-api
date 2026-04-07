using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.Api.Domain.Repositories;

public interface IBlockCypherRepository
{
    public void Save(BlockCypher record);
    public List<BlockCypher> GetPageFor(string coin, int page, int pageSize);
}
