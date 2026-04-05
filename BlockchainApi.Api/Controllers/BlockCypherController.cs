using Microsoft.AspNetCore.Mvc;
using BlockchainApi.Api.Domain.Models;
using BlockchainApi.Api.Domain;

namespace BlockchainApi.Api.Controllers;

[ApiController]
[Route("api/blockcypher/v1")]
public class BlockCypherController : ControllerBase
{
    private const string BTC = "btc";
    private const string ETH = "eth";
    private const string LTC = "ltc";
    private const string DASH = "dash";

    private static readonly List<string> Coins = new() { BTC, ETH, LTC, DASH };
    private IBlockCypherRepository _repository;

    public BlockCypherController(IBlockCypherRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public IActionResult GetCoin(string coin)
    {
        if (IsValid(coin))
            return BadRequest($"Invalid coin: {coin}. Valid coins are: {string.Join(", ", Coins)}");

        try
        {
            using var http = new HttpClient();

            var response = http.GetAsync($"https://api.blockcypher.com/v1/{coin}/main").Result;
            var result = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, $"Error fetching data from BlockCypher API: {result}");

            var record = BlockCypher.FromJson(coin, result);

            _repository.Save(record);
            return Ok(BlockcypherSnapshotDto.FromRecord(record));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching data from BlockCypher API: {ex.Message}");
        }
    }

    /// <summary>
    /// Returns the history of block information requested for the specified coin.
    /// </summary> 
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>List of historical block information in JSON format.</returns>
    [HttpGet("{coin}/history")]
    public IActionResult GetHistory(string coin)
    {
        try
        {
            if (IsValid(coin))
                return BadRequest($"Invalid coin: {coin}. Valid coins are: {string.Join(", ", Coins)}");

            if (!_repository.TryGetHistory(coin, out var history) || history.Count == 0)
                return NotFound($"No history found for coin: {coin}");

            var snapshotDtos = history.Select(BlockcypherSnapshotDto.FromRecord).ToList();

            return Ok(snapshotDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, $"Error retrieving history for coin: {coin}");
        }
    }

    private static bool IsValid(string coin) => !Coins.Contains(coin);
}
