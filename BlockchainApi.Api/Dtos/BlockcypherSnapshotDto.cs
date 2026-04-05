using System.Text.Json;
using BlockchainApi.Api.Domain.Models;

public class BlockcypherSnapshotDto
{
    public string Chain { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public JsonElement Data { get; set; }

    public static BlockcypherSnapshotDto FromRecord(BlockCypher entity)
    {
        return new BlockcypherSnapshotDto
        {
            Chain = entity.Coin,
            CreatedAt = entity.CreatedAt,
            Data = JsonSerializer.Deserialize<JsonElement>(entity.RawData ?? "{}")
        };
    }
}
