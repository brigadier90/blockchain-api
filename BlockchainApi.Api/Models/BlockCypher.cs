namespace BlockchainApi.Api.Models;

public record BlockCypher
{
    public DateTime CreatedAt { get; set; }
    public string? RawData { get; set; }
}
