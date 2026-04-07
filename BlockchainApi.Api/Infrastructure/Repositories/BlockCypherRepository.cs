using BlockchainApi.Api.Domain.Models;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainApi.Api.Infrastructure.Repositories;

public class BlockCypherRepository : IBlockCypherRepository
{
    private readonly BlockCypherContext _context;
    private readonly ILogger<BlockCypherRepository> _logger;

    public BlockCypherRepository(BlockCypherContext context, ILogger<BlockCypherRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void Save(BlockCypher record)
    {
        _logger.LogInformation("Saving block record for coin {Coin} at {CreatedAt}", record.Coin, record.CreatedAt);
        _context.BlockCyphers.Add(record);
        _context.SaveChanges();
    }

    public bool TryGetAllFor(string coin, out List<BlockCypher> history)
    {
        _logger.LogInformation("Querying history for coin {Coin}", coin);
        history = _context.BlockCyphers
            .Where(x => x.Coin == coin)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToList();

        return history.Count > 0;
    }
}