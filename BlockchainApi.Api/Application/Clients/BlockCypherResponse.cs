namespace BlockchainApi.Api.Application.Clients;

public sealed record BlockCypherResponse
{
    public int StatusCode { get; init; }
    public string Content { get; init; } = string.Empty;
    public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;
}
