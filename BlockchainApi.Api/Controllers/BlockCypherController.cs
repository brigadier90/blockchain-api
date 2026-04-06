using Microsoft.AspNetCore.Mvc;
using BlockchainApi.Api.Domain.Models;
using BlockchainApi.Api.Domain;
using MediatR;
using BlockchainApi.Api.Application.Commands;

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
    private IMediator _mediator;
    private IBlockCypherRepository _repository;

    public BlockCypherController(IMediator mediator, IBlockCypherRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public async Task<IActionResult> GetCoin(string coin)
    {
        try
        {
            var result = await _mediator.Send(new GetBlockCypherCommand(coin));

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(BlockcypherSnapshotDto.FromRecord(result.Value!));
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
            if (!IsValid(coin))
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

    private static bool IsValid(string coin) => Coins.Contains(coin);
}
