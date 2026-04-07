namespace BlockchainApi.Api.Domain.Models;

public record BlockCypher
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Coin { get; set; } = null!;
    public string RawData { get; set; } = null!;

    public static BlockCypher FromJson(string coin, string json)
    {
        return new BlockCypher
        {
            CreatedAt = DateTime.UtcNow,
            Coin = coin,
            RawData = json
        };
    }
}
