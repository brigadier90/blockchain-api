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

    public async Task SaveAsync(BlockCypher record)
    {
        _logger.LogInformation("Saving block record for coin {Coin} at {CreatedAt}", record.Coin, record.CreatedAt);
        _context.BlockCyphers.Add(record);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Block record persisted for coin {Coin} with ID {Id}", record.Coin, record.Id);
    }

    public async Task<List<BlockCypher>> GetAllAsync(string coin, int? page = null, int? pageSize = null)
    {
        _logger.LogInformation("Querying history for coin {Coin} page {Page} pageSize {PageSize}", coin, page, pageSize);

        var query = _context.BlockCyphers
            .Where(x => x.Coin == coin)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking();

        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }
}