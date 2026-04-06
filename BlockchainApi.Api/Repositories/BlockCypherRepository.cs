using BlockchainApi.Api.Domain;
using BlockchainApi.Api.Domain.Models;

namespace BlockchainApi.Api.Repositories;

public class BlockCypherRepository : IBlockCypherRepository
{
    private readonly Dictionary<string, List<BlockCypher>> _storage = new();

    public void Save(BlockCypher record)
    {
        if (!_storage.ContainsKey(record.Coin))
            _storage[record.Coin] = new List<BlockCypher>();

        _storage[record.Coin].Add(record);
    }

    public bool TryGetAllFor(string coin, out List<BlockCypher> history)
    {
        if (_storage.TryGetValue(coin, out var stored))
        {
            history = [.. stored];
            return true;
        }

        history = new List<BlockCypher>();
        return false;
    }
}