namespace BlockchainApi.Api.Models;

public record BlockCypher
{
    public DateTime CreatedAt { get; set; }
    public string Coin { get; set; } = null!;
    public string RawData { get; set; } = null!;
}
