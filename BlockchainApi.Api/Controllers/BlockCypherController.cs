using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using BlockchainApi.Api.Application.Commands;
using BlockchainApi.Api.Application.Queries;

namespace BlockchainApi.Api.Controllers;

[ApiController]
[Route("api/blockcypher/v1")]
public class BlockCypherController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BlockCypherController> _logger;

    public BlockCypherController(IMediator mediator, ILogger<BlockCypherController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public async Task<IActionResult> GetCoin(string coin)
    {
        _logger.LogInformation("Received request for latest block data for coin {Coin}", coin);

        var result = await _mediator.Send(new GetBlockCypherCommand(coin));

        if (!result.IsSuccess)
        {
            _logger.LogWarning("GetCoin failed for coin {Coin}: {Error}", coin, result.Error);
            return StatusCode(result.StatusCode, result.Error);
        }

        _logger.LogInformation("GetCoin succeeded for coin {Coin}", coin);
        return Ok(result.Value);
    }

    /// <summary>
    /// Returns the history of block information requested for the specified coin.
    /// </summary> 
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>List of historical block information in JSON format.</returns>
    [HttpGet("{coin}/history")]
    public async Task<IActionResult> GetHistory(string coin)
    {
        _logger.LogInformation("Received request for history data for coin {Coin}", coin);

        var result = await _mediator.Send(new GetBlockCypherHistoryQuery(coin));

        if (!result.IsSuccess)
        {
            _logger.LogWarning("GetHistory failed for coin {Coin}: {Error}", coin, result.Error);
            return StatusCode(result.StatusCode, result.Error);
        }

        _logger.LogInformation("GetHistory succeeded for coin {Coin}, returned {Count} snapshots", coin, result.Value?.Count ?? 0);
        return Ok(result.Value);
    }
}
