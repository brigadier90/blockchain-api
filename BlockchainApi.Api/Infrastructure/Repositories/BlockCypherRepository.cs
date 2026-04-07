using BlockchainApi.Api.Domain.Models;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlockchainApi.Api.Infrastructure.Repositories;

public class BlockCypherRepository : IBlockCypherRepository
{
    private readonly BlockCypherContext _context;

    public BlockCypherRepository(BlockCypherContext context)
    {
        _context = context;
    }

    public void Save(BlockCypher record)
    {
        _context.BlockCyphers.Add(record);
        _context.SaveChanges();
    }

    public bool TryGetAllFor(string coin, out List<BlockCypher> history)
    {
        history = _context.BlockCyphers
            .Where(x => x.Coin == coin)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToList();

        return history.Count > 0;
    }
}