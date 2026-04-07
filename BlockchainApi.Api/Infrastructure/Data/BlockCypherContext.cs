using BlockchainApi.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockchainApi.Api.Infrastructure.Data;

public class BlockCypherContext : DbContext
{
    public BlockCypherContext(DbContextOptions<BlockCypherContext> options)
        : base(options)
    {
    }

    public DbSet<BlockCypher> BlockCyphers => Set<BlockCypher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockCypher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Coin).IsRequired();
            entity.Property(e => e.RawData).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
