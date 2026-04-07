using BlockchainApi.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockchainApi.Api.Infrastructure.Persistence;

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

            // Index on Coin for faster filtering
            entity.HasIndex(e => e.Coin);

            // Index on CreatedAt for sorting performance
            entity.HasIndex(e => e.CreatedAt);

            // Composite index for the common query pattern: WHERE Coin = ? ORDER BY CreatedAt DESC
            entity.HasIndex(e => new { e.Coin, e.CreatedAt })
                .IsDescending(false, true);
        });

        base.OnModelCreating(modelBuilder);
    }
}
