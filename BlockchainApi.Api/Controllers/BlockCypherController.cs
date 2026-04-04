using Microsoft.AspNetCore.Mvc;
using BlockchainApi.Api.Models;

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
    private static readonly Dictionary<string, List<BlockCypher>> History = new();

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public IActionResult GetCoin(string coin)
    {
        if (!Coins.Contains(coin))
            return BadRequest($"Invalid coin: {coin}. Valid coins are: {string.Join(", ", Coins)}");

        try
        {
            using var http = new HttpClient();

            var response = http.GetAsync($"https://api.blockcypher.com/v1/{coin}/main").Result;
            var result = response.Content.ReadAsStringAsync().Result;

            if (!History.ContainsKey(coin))
                History[coin] = new List<BlockCypher>();

            var record = new BlockCypher
            {
                CreatedAt = DateTime.UtcNow,
                RawData = result
            };

            History[coin].Add(record);

            return Content(result, "application/json");
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
            if (!Coins.Contains(coin))
                return BadRequest($"Invalid coin: {coin}. Valid coins are: {string.Join(", ", Coins)}");

            if (!History.ContainsKey(coin))
                return NotFound($"No history found for coin: {coin}");

            return Ok(History[coin]);
        }
        catch (System.Exception)
        {
            return StatusCode(500, $"Error retrieving history for coin: {coin}");
        }
    }
}
