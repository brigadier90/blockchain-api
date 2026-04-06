using Microsoft.AspNetCore.Mvc;
using BlockchainApi.Api.Domain;
using MediatR;
using BlockchainApi.Api.Application.Commands;
using BlockchainApi.Api.Application.Queries;

namespace BlockchainApi.Api.Controllers;

[ApiController]
[Route("api/blockcypher/v1")]
public class BlockCypherController : ControllerBase
{
    private IMediator _mediator;

    public BlockCypherController(IMediator mediator, IBlockCypherRepository repository)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the latest block information from BlockCypher API for the specified coin.
    /// </summary>
    /// <param name="coin">The cryptocurrency coin (e.g., btc, eth, ltc, dash).</param>
    /// <returns>Latest block information in JSON format.</returns>
    [HttpGet("{coin}/main")]
    public async Task<IActionResult> GetCoin(string coin)
    {
        var result = await _mediator.Send(new GetBlockCypherCommand(coin));

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result.Error);

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
        var result = await _mediator.Send(new GetBlockCypherHistoryQuery(coin));

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result.Error);

        return Ok(result.Value);
    }
}
