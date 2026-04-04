using Microsoft.AspNetCore.Mvc;

namespace BlockchainApi.Api.Controllers;

[ApiController]
[Route("api/blockcypher/v1")]
public class BlockCypherController : ControllerBase
{
    private static readonly List<string> Coins = new() { "btc", "eth", "ltc", "dash" };

    private static readonly Dictionary<string, List<string>> History = new();

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public IActionResult GetCoin(string coin)
    {
        using var http = new HttpClient();

        var response = http.GetAsync($"https://api.blockcypher.com/v1/{coin}/main").Result;
        var result = response.Content.ReadAsStringAsync().Result;

        if (!History.ContainsKey(coin))
            History[coin] = new List<string>();

        History[coin].Add(result);

        return Content(result, "application/json");
    }

    /// <summary>
    /// Returns the history of block information requested for the specified coin.
    /// </summary> 
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>List of historical block information in JSON format.</returns>
    [HttpGet("{coin}/history")]
    public IActionResult GetHistory(string coin)
    {
        if (!History.ContainsKey(coin))
            return NotFound($"No history found for coin: {coin}");

        return Ok(History[coin]);
    }
}
