using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.Api.Domain.Repositories;

public interface IBlockCypherRepository
{
    public void Save(BlockCypher record);
    public bool TryGetAllFor(string coin, out List<BlockCypher> history);
}
