using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.Api.Domain;

public interface IBlockCypherRepository
{
    public void Save(BlockCypher record);
    public bool TryGetHistory(string coin, out List<BlockCypher> history);
}
