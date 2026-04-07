using BlockchainApi.Api.Application.Clients;
using BlockchainApi.Api.Application.Common;
using BlockchainApi.Api.Domain.Repositories;
using BlockchainApi.Api.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using BlockchainApi.Api.Dtos;

namespace BlockchainApi.Api.Application.Commands;

public record GetBlockCypherCommand(string Coin) : IRequest<Result<BlockcypherSnapshotDto>>;

public class GetBlockCypherCommandHandler : IRequestHandler<GetBlockCypherCommand, Result<BlockcypherSnapshotDto>>
{
    private readonly IBlockCypherClient _client;
    private readonly IBlockCypherRepository _repository;
    private readonly ILogger<GetBlockCypherCommandHandler> _logger;
    
    public GetBlockCypherCommandHandler(IBlockCypherClient client, IBlockCypherRepository repository, ILogger<GetBlockCypherCommandHandler> logger)
    {
        _client = client;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<BlockcypherSnapshotDto>> Handle(GetBlockCypherCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBlockCypherCommand for coin {Coin}", request.Coin);
        if (!Constants.ValidCoins.Contains(request.Coin))
        {
            _logger.LogWarning("Invalid coin requested: {Coin}", request.Coin);
            return Result<BlockcypherSnapshotDto>.Failure(
                $"Invalid coin: {request.Coin}. Valid coins are: {string.Join(", ", Constants.ValidCoins)}",
                StatusCodes.Status400BadRequest);
        }

        _logger.LogInformation("Fetching block data for coin {Coin}", request.Coin);
        var response = await _client.FetchAsync(request.Coin, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("BlockCypher API returned non-success status {StatusCode} for coin {Coin}", response.StatusCode, request.Coin);
            return Result<BlockcypherSnapshotDto>.Failure(
                $"Error fetching data from BlockCypher API: {response.Content}",
                response.StatusCode);
        }

        var record = BlockCypher.FromJson(request.Coin, response.Content);
        await _repository.SaveAsync(record);
        _logger.LogInformation("Saved block data for coin {Coin} at {CreatedAt}", request.Coin, record.CreatedAt);

        var dto = BlockcypherSnapshotDto.FromRecord(record);
        return Result<BlockcypherSnapshotDto>.Success(dto);
    }
}
